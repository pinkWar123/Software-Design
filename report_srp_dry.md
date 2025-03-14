# Refactoring : Applying DRY and SRP Principles

## Introduction

In our ASP.NET Core Web API project, the `StudentService` class currently handles various responsibilities, including business logic and validation. To adhere to the **DRY (Don't Repeat Yourself)** and **SRP (Single Responsibility Principle)** principles, we will refactor the class by extracting validation logic into a separate service.

## Current Issues

The `StudentService` class is responsible for:

- Importing and exporting students via CSV and JSON.
- Performing CRUD operations.
- Validating phone numbers and email addresses.

```csharp
    public interface IStudentService
    {
        Task ImportFromCsv(Stream stream);
        Task ImportFromJson(Stream stream);
        Task<byte[]> ExportToCsv();
        Task<byte[]> ExportToJson();

        Task<Student> CreateNewStudent(CreateStudentDto student);
        Task<Student?> GetStudentById(int id);
        Task<Student?> UpdateStudent(int studentId,UpdateStudentDto student);
        Task<bool> DeleteStudent(int id);
        bool ValidatePhone(string phoneNumber);
        bool ValidateEmail(string email);
    }
```

The validation logic (`ValidatePhone` and `ValidateEmail`) does not belong to the business logic layer. Keeping validation inside the service layer reduces reusability and makes testing harder.

## Refactored Solution

We will introduce a new `IValidationService` to handle all validation-related concerns.

### 1. Create a New Validation Service

```csharp
public interface IStudentValidationService
{
    bool ValidatePhone(string phoneNumber);
    bool ValidateEmail(string email);
}

public class StudentValidationService : IValidationService
{
    public bool ValidatePhone(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^\+?[1-9]\d{1,14}$");
    }

    public bool ValidateEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
```

### 2. Modify StudentService to Use ValidationService

```csharp
public class StudentService : IStudentService
{
    private readonly IValidationService _validationService;

    public StudentService(IValidationService validationService)
    {
        _validationService = validationService;
    }

    public bool ValidatePhone(string phoneNumber)
    {
        return _validationService.ValidatePhone(phoneNumber);
    }

    public bool ValidateEmail(string email)
    {
        return _validationService.ValidateEmail(email);
    }
}
```

### 3. Duplicate code that violates DRY

```csharp
            var shouldPhoneValidationBeApplied = await _configurationRepository.GetConfigurationByKeyAsync("AllowedPhonePattern");
            if (shouldPhoneValidationBeApplied != null && shouldPhoneValidationBeApplied.IsActive)
            {
                if (!_validator.ValidatePhone(student.PhoneNumber))
                    throw new Exception("Số điện thoại không hợp lệ");
            }

            var shouldEmailValidationBeApplied = await _configurationRepository.GetConfigurationByKeyAsync("AllowedEmailDomain");
            if (shouldEmailValidationBeApplied != null && shouldEmailValidationBeApplied.IsActive)
            {
                if (!_validator.ValidateEmail(student.Email))
                    throw new Exception("Email không hợp lệ");
            }
            if(student.StudentId != studentId)
            {
                var anotherStudent = await GetStudentById(student.StudentId);
                if (anotherStudent != null)
                    throw new Exception("Mã sinh viên đã tồn tại");
            }
```

These lines of code are used for more than one time in Create and Update method. It checks for phone, email domain constraint as well as the existence of the student. So I decided to move it to validationService as well to remove duplicacy:

```csharp
public interface IValidationService
{
    bool ValidatePhone(string phoneNumber);
    bool ValidateEmail(string email);
    Task ValidateStudentAsync(Student student, int studentId);
}
```

Then in StudentService we just need to do like this:

```csharp
        public async Task<Student> CreateNewStudent(CreateStudentDto student)
        {
            await _validator.ValidateStudentAsync(student.PhoneNumber, student.Email);
            var existingStudent = await GetStudentById(student.StudentId);
            if (existingStudent != null)
                throw new Exception("Mã sinh viên đã tồn tại");

            var newStudent = new Student()
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Batch = student.Batch,
                Address = student.Address,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                StatusId = student.StatusId,
                ProgramId = student.ProgramId,
                FacultyId = student.FacultyId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Students.AddAsync(newStudent);
            await _context.SaveChangesAsync();
            return newStudent;
        }
```

### 4. Dependency Injection Configuration (Startup.cs / Program.cs)

```csharp
services.AddScoped<IValidationService, ValidationService>();
services.AddScoped<IStudentService, StudentService>();
```

## Benefits of This Refactoring

- **SRP Compliance:** The `StudentService` class now focuses solely on business logic, while `ValidationService` handles validation.
- **DRY Compliance:** Any other services needing validation can now use `ValidationService`, avoiding redundant implementations.
- **Improved Testability:** We can now test validation separately, improving modularity.

## Alternative Approach

Instead of creating a dedicated `ValidationService`, we could use **FluentValidation**, a popular .NET library for validation. Example:

```csharp
public class StudentValidator : AbstractValidator<CreateStudentDto>
{
    public StudentValidator()
    {
        RuleFor(s => s.PhoneNumber).Matches(@"^\+?[1-9]\d{1,14}$");
        RuleFor(s => s.Email).EmailAddress();
    }
}
```

Using FluentValidation keeps validation separate and provides additional features like dependency injection, localization, and model validation.

## Conclusion

Refactoring `StudentService` improves maintainability, reusability, and testability. By implementing `ValidationService` or leveraging FluentValidation, we ensure that the project follows clean architecture principles.

# Refactoring Document Generation Using the Factory Pattern

## Introduction

In our ASP.NET Core Web API project, we need a way to generate different document formats, such as **HTML, Markdown, and PDF**. Initially, document generation was handled in a way that violated the **Open-Closed Principle (OCP)** and **Single Responsibility Principle (SRP)**.

To improve maintainability and scalability, we refactored the code using the **Factory Pattern**.

## The Problem with the Old Implementation

The previous implementation created document generators in a way that required modifying the service every time a new format was introduced. Here’s an example of the **bad approach**:

### ❌ **Old Version (Tightly Coupled, Violation of OCP & SRP)**

```csharp
public class DocumentService
{
    private readonly SynchronizedConverter _converter;

    public DocumentService(SynchronizedConverter converter)
    {
        _converter = converter;
    }

    public byte[] GenerateDocument(DocumentType format, StudentDocumentDto data)
    {
        switch (format)
        {
            case DocumentType.HTML:
                return new HtmlDocumentGenerator().GenerateDocument(data).Result;
            case DocumentType.Markdown:
                return new MarkdownDocumentGenerator().GenerateDocument(data).Result;
            case DocumentType.PDF:
                var htmlGenerator = new HtmlDocumentGenerator();
                return new PDFDocumentGenerator(htmlGenerator, _converter).GenerateDocument(data).Result;
            default:
                throw new NotSupportedException("Document format not supported.");
        }
    }
}
```

### 🚨 **Issues with This Approach:**

- **Tightly Coupled:** The `DocumentService` directly depends on specific document generators.
- **Violates OCP:** Every time a new document format is added (e.g., DOCX), the `switch` statement must be modified.
- **Difficult to Test:** Unit testing is harder due to direct dependencies on specific classes.

## The Refactored Solution: Factory Pattern

To fix these issues, we implemented the **Factory Pattern**, which allows us to create document generators dynamically.

### ✅ **New Version (Factory Pattern, OCP & SRP Compliant)**

#### **1. Define the Document Generator Interface**

```csharp
public interface IDocumentGenerator
{
    Task<byte[]> GenerateDocument(StudentDocumentDto data);
}
```

#### **2. Implement Concrete Document Generators**

Each document type now has its own class that implements `IDocumentGenerator`.

Example:

```csharp
public class HtmlDocumentGenerator : IDocumentGenerator
{
    public Task<byte[]> GenerateDocument(StudentDocumentDto data)
    {
        string htmlContent = "<html><body>...Student Info...</body></html>";
        return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(htmlContent));
    }
}
```

```csharp
public class MarkdownDocumentGenerator : IDocumentGenerator
{
    public Task<byte[]> GenerateDocument(StudentDocumentDto data)
    {
        string markdownContent = "**Student Info**\n- Name: " + data.StudentName;
        return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(markdownContent));
    }
}
```

```csharp
public class PDFDocumentGenerator : IDocumentGenerator
{
    private readonly HtmlDocumentGenerator _htmlGenerator;
    private readonly SynchronizedConverter _converter;

    public PDFDocumentGenerator(HtmlDocumentGenerator htmlGenerator, SynchronizedConverter converter)
    {
        _htmlGenerator = htmlGenerator;
        _converter = converter;
    }

    public async Task<byte[]> GenerateDocument(StudentDocumentDto data)
    {
        byte[] htmlBytes = await _htmlGenerator.GenerateDocument(data);
        string htmlContent = System.Text.Encoding.UTF8.GetString(htmlBytes);
        var doc = new HtmlToPdfDocument { Objects = { new ObjectSettings { HtmlContent = htmlContent } } };
        return _converter.Convert(doc);
    }
}
```

#### **3. Implement the Factory Class**

```csharp
public class DocumentGeneratorFactory : IDocumentGeneratorFactory
{
    private readonly SynchronizedConverter _converter;

    public DocumentGeneratorFactory(SynchronizedConverter converter)
    {
        _converter = converter;
    }

    public IDocumentGenerator CreateDocumentGenerator(DocumentType format)
    {
        return format switch
        {
            DocumentType.HTML => new HtmlDocumentGenerator(),
            DocumentType.Markdown => new MarkdownDocumentGenerator(),
            DocumentType.PDF => new PDFDocumentGenerator(new HtmlDocumentGenerator(), _converter),
            _ => throw new NotSupportedException("Document format not supported.")
        };
    }
}
```

#### **4. Use the Factory in DocumentService**

```csharp
public class DocumentService
{
    private readonly IDocumentGeneratorFactory _documentGeneratorFactory;

    public DocumentService(IDocumentGeneratorFactory documentGeneratorFactory)
    {
        _documentGeneratorFactory = documentGeneratorFactory;
    }

    public async Task<byte[]> GenerateDocument(DocumentType format, StudentDocumentDto data)
    {
        var generator = _documentGeneratorFactory.CreateDocumentGenerator(format);
        return await generator.GenerateDocument(data);
    }
}
```

## Benefits of This Refactoring

✅ **Open-Closed Principle (OCP) Compliance:** Adding a new document format no longer requires modifying the `DocumentService`. Simply create a new class and register it in the factory.

✅ **Single Responsibility Principle (SRP) Compliance:** Each document generator is now responsible only for generating documents of its type.

✅ **Easier Unit Testing:** Individual document generators and the factory can be tested separately.

✅ **More Maintainable and Scalable:** We can introduce new formats without breaking existing code.

## Conclusion

By refactoring document generation using the **Factory Pattern**, we achieved a more flexible, maintainable, and scalable architecture. The new approach follows SOLID principles and significantly improves testability. This ensures that adding new document formats in the future will be seamless and without unnecessary code modifications.

# Refactoring StudentCreateModal and StudentUpdateModal for DRY and SRP Compliance

## Introduction

The `StudentCreateModal.tsx` and `StudentUpdateModal.tsx` components in our React project handle the creation and updating of student records. However, these components contain a significant amount of duplicated code, violating the **DRY (Don't Repeat Yourself)** principle. Additionally, they mix multiple responsibilities, making them harder to maintain and extend, thus violating the **Single Responsibility Principle (SRP)**.

To improve maintainability and scalability, we refactored these components by extracting reusable logic into a shared form component.

## Identified Issues

- **Code Duplication:** Both components contain nearly identical form structures, including inputs, validation rules, and UI elements.
- **SRP Violation:** Each component is responsible for both rendering the modal and handling form logic.
- **Difficult Maintenance:** Any change to the student form (e.g., adding a new field) requires updating both files separately.

## Refactored Solution: Extracting a Reusable `StudentForm` Component

### **1. Create a Shared StudentForm Component**

We extract the common form logic into a reusable component.

```tsx
import React from "react";
import { Form, Input, Select, DatePicker, Button, Checkbox } from "antd";
import { notificationOptions } from "../constants/notificationOptions";
import IProgram from "../models/Program";
import IStatus from "../models/Status";
import IFaculty from "../models/Faculty";
import dayjs from "dayjs";

interface StudentFormProps {
  form: any;
  studyPrograms: IProgram[];
  statuses: IStatus[];
  faculties: IFaculty[];
  onSubmit: (values: any) => Promise<void>;
  isUpdate?: boolean;
  initialValues?: any;
}

const StudentForm: React.FC<StudentFormProps> = ({
  form,
  studyPrograms,
  statuses,
  faculties,
  onSubmit,
  isUpdate,
  initialValues,
}) => {
  return (
    <Form
      form={form}
      onFinish={onSubmit}
      layout="vertical"
      initialValues={initialValues}
    >
      <Form.Item
        name="studentId"
        label="Mã số sinh viên"
        rules={[{ required: true, message: "Vui lòng nhập mã số sinh viên!" }]}
      >
        <Input disabled={isUpdate} />
      </Form.Item>
      <Form.Item
        name="fullName"
        label="Họ và tên"
        rules={[{ required: true, message: "Vui lòng nhập họ và tên!" }]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        name="dateOfBirth"
        label="Ngày sinh"
        rules={[{ required: true, message: "Vui lòng chọn ngày sinh!" }]}
      >
        <DatePicker format="DD/MM/YYYY" />
      </Form.Item>
      <Form.Item
        name="gender"
        label="Giới tính"
        rules={[{ required: true, message: "Vui lòng chọn giới tính!" }]}
      >
        <Select>
          <Select.Option value="Nam">Nam</Select.Option>
          <Select.Option value="Nữ">Nữ</Select.Option>
          <Select.Option value="Khác">Khác</Select.Option>
        </Select>
      </Form.Item>
      <Form.Item
        name="facultyId"
        label="Khoa"
        rules={[{ required: true, message: "Vui lòng chọn khoa!" }]}
      >
        <Select>
          {faculties.map((faculty) => (
            <Select.Option key={faculty.id} value={faculty.id}>
              {faculty.name}
            </Select.Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item
        name="batch"
        label="Khóa"
        rules={[{ required: true, message: "Vui lòng nhập khóa!" }]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        name="address"
        label="Địa chỉ"
        rules={[{ required: true, message: "Vui lòng nhập địa chỉ!" }]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        name="programId"
        label="Chương trình học"
        rules={[{ required: true, message: "Vui lòng chọn chương trình học!" }]}
      >
        <Select>
          {studyPrograms.map((program) => (
            <Select.Option key={program.id} value={program.id}>
              {program.name}
            </Select.Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item
        name="statusId"
        label="Trạng thái"
        rules={[{ required: true, message: "Vui lòng chọn trạng thái!" }]}
      >
        <Select>
          {statuses.map((status) => (
            <Select.Option key={status.id} value={status.id}>
              {status.name}
            </Select.Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item
        name="email"
        label="Email"
        rules={[
          { required: true, message: "Vui lòng nhập email!" },
          { type: "email", message: "Email không hợp lệ!" },
        ]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        name="phoneNumber"
        label="Số điện thoại"
        rules={[{ required: true, message: "Vui lòng nhập số điện thoại!" }]}
      >
        <Input />
      </Form.Item>
      <Form.Item name="subscribeToNotifications" label="Nhận thông báo qua">
        <Checkbox.Group options={notificationOptions} />
      </Form.Item>
      <Form.Item>
        <Button type="primary" htmlType="submit">
          {isUpdate ? "Cập nhật" : "Tạo mới"}
        </Button>
      </Form.Item>
    </Form>
  );
};

export default StudentForm;
```

### **2. Refactor StudentCreateModal to Use StudentForm**

```tsx
const StudentCreateModal = ({
  studyPrograms,
  statuses,
  faculties,
  updateStudents,
}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [form] = Form.useForm();

  const handleSubmit = async (values) => {
    await callCreateStudent(values);
    message.success("Tạo học sinh mới thành công!");
    setIsModalOpen(false);
    await updateStudents();
  };

  return (
    <>
      <Button type="primary" onClick={() => setIsModalOpen(true)}>
        Tạo học sinh mới
      </Button>
      <Modal
        title="Tạo học sinh mới"
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
      >
        <StudentForm
          form={form}
          studyPrograms={studyPrograms}
          statuses={statuses}
          faculties={faculties}
          onSubmit={handleSubmit}
        />
      </Modal>
    </>
  );
};
```

## Benefits of This Refactoring

✅ **DRY Compliance:** The shared `StudentForm` component eliminates code duplication.

✅ **SRP Compliance:** `StudentCreateModal` and `StudentUpdateModal` now only handle modal state, while the `StudentForm` handles form logic.

✅ **Easier Maintenance:** Any future changes to the form fields only need to be updated in one place.

## Conclusion

By extracting a reusable `StudentForm` component, we improved code maintainability, reduced duplication, and enhanced scalability. This refactor ensures that both modals follow **DRY** and **SRP** principles, making the codebase more robust and extensible.
