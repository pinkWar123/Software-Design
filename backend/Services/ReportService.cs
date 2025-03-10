using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Document;
using backend.Repositories;
using backend.Services.DocumentGeneration;

namespace backend.Services
{
    public class ReportService : IReportService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IDocumentGeneratorFactory _factory;
        public ReportService(
            IStudentRepository studentRepository,
            IDocumentGeneratorFactory factory)
        {
            _studentRepository = studentRepository;
            _factory = factory;
        }
        public async Task<ReportFile?> GenerateReportForStudent(int studentId,  DocumentType format, string reason, string expiredDate)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId);
            if(student == null)
            {
                throw new Exception("Không tìm thấy học sinh");
            }

            var documentData = new StudentDocumentDto
            {
                UniversityName = "Trường Đại học khoa học tự nhiên",
                Address = "Nguyễn Văn Cừ",
                Phone = "0123456789",
                Email = student.Email,
                StudentName = student.FullName,
                StudentId = student.StudentId,
                Dob = student.DateOfBirth.ToString(),
                Gender = student.Gender,
                Faculty = student.Faculty.Name,
                Program = student.Program.Name,
                Course = student.Batch,
                IssueDate = DateTime.UtcNow.ToString(),
                Status = student.Status.Name,
                Reason = reason,
                ValidUntil = expiredDate
            };

            var documentGenerator = _factory.CreateDocumentGenerator(format);
            Byte[] fileBytes = await documentGenerator.GenerateDocument(documentData);

            var extension = ContentTypeGenerator.GetExtension(format);
            var contentType = ContentTypeGenerator.GetContentType(format);
            var response = new ReportFile
            {
                Content = fileBytes,
                ContentType = contentType,
                FileName = $"{student.StudentId}-report.{extension}"
            };

            return response;
        }
    }
}