using System;
using System.Threading.Tasks;
using backend.Data;
using backend.Dtos.Student;
using backend.Entities;
using backend.Repositories;
using backend.Services;
using backend.Settings;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace backend.Tests.Services
{
    public class StudentService_UpdateStudentTests
    {
        private readonly Mock<IApplicationDbContext> _context;
        private readonly IStudentService _studentService;
        private readonly IOptions<StudentSettings> _studentSettings;
        private readonly IOptions<StudentStatusTransitions> _studentStatusTransitions;
        private readonly IStatusRepository _statusRepository;

        public StudentService_UpdateStudentTests()
        {
            // Setup in-memory database
            _context = new Mock<IApplicationDbContext>();

            // Configure student settings with a phone regex that only accepts digits and a valid email domain.
            var settings = new StudentSettings { PhoneNumber = @"^\d+$", EmailDomain = "@domain.com" };
            _studentSettings = Options.Create(settings);

            // Setup empty transitions (not used in update)
            var transitions = new StudentStatusTransitions();
            _studentStatusTransitions = Options.Create(transitions);

            // Use a dummy status repository (not used by UpdateStudent)
            var statusRepoMock = new Mock<IStatusRepository>();
            _statusRepository = statusRepoMock.Object;

            _studentService = new StudentService(_context.Object, _studentSettings, _studentStatusTransitions, _statusRepository);
        }

        [Fact]
        public async Task UpdateStudent_Should_ThrowException_When_StudentNotFound()
        {
            // Arrange: No student with ID 100 exists.
            var updateDto = new UpdateStudentDto
            {
                StudentId = 100,
                FullName = "Updated Name",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Gender = "Female",
                Batch = "2021",
                Address = "New Address",
                Email = "update@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            _context.Setup(c => c.Students.FindAsync(updateDto.StudentId)).ReturnsAsync((Student?)null);

            // Act & Assert
            Func<Task> act = async () => await _studentService.UpdateStudent(100, updateDto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Không tìm thấy sinh viên");
        }

        [Fact]
        public async Task UpdateStudent_Should_ThrowException_When_PhoneIsInvalid()
        {
            // Arrange: Create an existing student.
            var student = new Student
            {
                StudentId = 10,
                FullName = "Original Name",
                DateOfBirth = DateTime.Now.AddYears(-22),
                Gender = "Male",
                Batch = "2020",
                Address = "Old Address",
                Email = "original@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            // Prepare update DTO with invalid phone (contains non-digits)
            var updateDto = new UpdateStudentDto
            {
                StudentId = 10,
                FullName = "Updated Name",
                DateOfBirth = DateTime.Now.AddYears(-22),
                Gender = "Male",
                Batch = "2020",
                Address = "Updated Address",
                Email = "updated@domain.com",
                PhoneNumber = "123-ABC-7890", // invalid phone number
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            _context.Setup(c => c.Students.FindAsync(updateDto.StudentId)).ReturnsAsync(student);

            // Act & Assert
            Func<Task> act = async () => await _studentService.UpdateStudent(10, updateDto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Số điện thoại không hợp lệ");
        }

        [Fact]
        public async Task UpdateStudent_Should_ThrowException_When_EmailIsInvalid()
        {
            // Arrange: Create an existing student.
            var student = new Student
            {
                StudentId = 20,
                FullName = "Original Name",
                DateOfBirth = DateTime.Now.AddYears(-22),
                Gender = "Male",
                Batch = "2020",
                Address = "Old Address",
                Email = "original@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            // Prepare update DTO with invalid email (wrong domain)
            var updateDto = new UpdateStudentDto
            {
                StudentId = 20,
                FullName = "Updated Name",
                DateOfBirth = DateTime.Now.AddYears(-22),
                Gender = "Male",
                Batch = "2020",
                Address = "Updated Address",
                Email = "updated@gmail.com", // invalid email domain
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };
            _context.Setup(c => c.Students.FindAsync(updateDto.StudentId)).ReturnsAsync(student);

            // Act & Assert
            Func<Task> act = async () => await _studentService.UpdateStudent(20, updateDto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Email không hợp lệ");
        }

        [Fact]
        public async Task UpdateStudent_Should_ThrowException_When_StudentIdChangedToDuplicate()
        {
            // Arrange: Create two students.
            var student1 = new Student
            {
                StudentId = 30,
                FullName = "Student One",
                DateOfBirth = DateTime.Now.AddYears(-22),
                Gender = "Male",
                Batch = "2020",
                Address = "Address One",
                Email = "one@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };
            _context.Setup(c => c.Students.FindAsync(student1.StudentId)).ReturnsAsync(student1);
            var student2 = new Student
            {
                StudentId = 31,
                FullName = "Student Two",
                DateOfBirth = DateTime.Now.AddYears(-21),
                Gender = "Female",
                Batch = "2020",
                Address = "Address Two",
                Email = "two@domain.com",
                PhoneNumber = "0987654321",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };
            _context.Setup(c => c.Students.FindAsync(student2.StudentId)).ReturnsAsync(student2);

            // Prepare update DTO for student1 attempting to change its ID to 31 (which already exists)
            var updateDto = new UpdateStudentDto
            {
                StudentId = 31, // trying to update student1 to have an existing student id
                FullName = "Updated Student One",
                DateOfBirth = student1.DateOfBirth,
                Gender = student1.Gender,
                Batch = student1.Batch,
                Address = "Updated Address One",
                Email = "updated@domain.com",
                PhoneNumber = "1234567890",
                StatusId = student1.StatusId,
                ProgramId = student1.ProgramId,
                FacultyId = student1.FacultyId,
            };

            // Act & Assert
            Func<Task> act = async () => await _studentService.UpdateStudent(30, updateDto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Mã sinh viên đã tồn tại");
        }

        [Fact]
        public async Task UpdateStudent_Should_UpdateStudent_When_ValidInput()
        {
            // Arrange: Create an existing student.
            var student = new Student
            {
                StudentId = 40,
                FullName = "Original Name",
                DateOfBirth = DateTime.Now.AddYears(-22),
                Gender = "Male",
                Batch = "2020",
                Address = "Old Address",
                Email = "original@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            // Prepare a valid update DTO. Note: Here, StudentId remains unchanged.
            var updateDto = new UpdateStudentDto
            {
                StudentId = 40, // same id as before
                FullName = "Updated Name",
                DateOfBirth = student.DateOfBirth.AddDays(1), // slight change
                Gender = "Male",
                Batch = "2021",
                Address = "Updated Address",
                Email = "updated@domain.com",
                PhoneNumber = "0987654321",
                StatusId = 2,
                ProgramId = 2,
                FacultyId = 2,
            };
            _context.Setup(c => c.Students.FindAsync(updateDto.StudentId)).ReturnsAsync(student);
            // Act
            var updatedStudent = await _studentService.UpdateStudent(40, updateDto);

            // Assert
            updatedStudent.Should().NotBeNull();
            updatedStudent.FullName.Should().Be("Updated Name");
            updatedStudent.DateOfBirth.Should().Be(updateDto.DateOfBirth);
            updatedStudent.Batch.Should().Be("2021");
            updatedStudent.Address.Should().Be("Updated Address");
            updatedStudent.Email.Should().Be("updated@domain.com");
            updatedStudent.PhoneNumber.Should().Be("0987654321");
            updatedStudent.StatusId.Should().Be(2);
            updatedStudent.ProgramId.Should().Be(2);
            updatedStudent.FacultyId.Should().Be(2);
        }
    }
}
