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
    public class StudentServiceTests
    {
        private readonly Mock<IApplicationDbContext> _context;
        private readonly IStudentService _studentService;
        private readonly IOptions<StudentSettings> _studentSettings;
        private readonly IOptions<StudentStatusTransitions> _studentStatusTransitions;
        private readonly IStatusRepository _statusRepository;

        public StudentServiceTests()
        {
            // Set up an in-memory database.
            
            _context = new Mock<IApplicationDbContext>();

            // Configure StudentSettings with a simple regex for phone and a valid email domain.
            var settings = new StudentSettings { PhoneNumber = @"^\d+$", EmailDomain = "@domain.com" };
            _studentSettings = Options.Create(settings);

            // For student status transitions, use default/empty settings as they are not used in CreateNewStudent.
            var transitions = new StudentStatusTransitions();
            _studentStatusTransitions = Options.Create(transitions);

            // Create a dummy status repository (not used by CreateNewStudent).
            var statusRepoMock = new Mock<IStatusRepository>();
            _statusRepository = statusRepoMock.Object;

            _studentService = new StudentService(_context.Object, _studentSettings, _studentStatusTransitions, _statusRepository);
        }

        [Fact]
        public async Task CreateNewStudent_Should_ThrowException_When_PhoneIsInvalid()
        {
            // Arrange: Use an invalid phone number (contains non-digit characters).
            var studentDto = new CreateStudentDto
            {
                StudentId = 1,
                FullName = "Test Student",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Gender = "Male",
                Batch = "2020",
                Address = "123 Street",
                Email = "test@domain.com",
                PhoneNumber = "123-ABC-7890", // Invalid phone
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            // Act & Assert
            Func<Task> act = async () => await _studentService.CreateNewStudent(studentDto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Số điện thoại không hợp lệ");
        }

        [Fact]
        public async Task CreateNewStudent_Should_ThrowException_When_EmailIsInvalid()
        {
            // Arrange: Use an invalid email domain.
            var studentDto = new CreateStudentDto
            {
                StudentId = 2,
                FullName = "Test Student",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Gender = "Male",
                Batch = "2020",
                Address = "123 Street",
                Email = "test@gmail.com", // Invalid email domain
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            // Act & Assert
            Func<Task> act = async () => await _studentService.CreateNewStudent(studentDto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Email không hợp lệ");
        }

        [Fact]
        public async Task CreateNewStudent_Should_ThrowException_When_StudentIdAlreadyExists()
        {
            // Arrange: Add a student with StudentId = 3.
            var existingStudent = new Student
            {
                StudentId = 3,
                FullName = "Existing Student",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Gender = "Male",
                Batch = "2020",
                Address = "123 Street",
                Email = "existing@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            // Attempt to add another student with the same StudentId.
            var studentDto = new CreateStudentDto
            {
                StudentId = 3, // Duplicate ID
                FullName = "Test Student",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Gender = "Male",
                Batch = "2020",
                Address = "123 Street",
                Email = "test@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };

            _context.Setup(c => c.Students.FindAsync(studentDto.StudentId)).ReturnsAsync(existingStudent);

            // Act & Assert
            Func<Task> act = async () => await _studentService.CreateNewStudent(studentDto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Mã sinh viên đã tồn tại");
        }

        [Fact]
        public async Task CreateNewStudent_Should_CreateStudent_When_ValidInput()
        {
            // Arrange: Create a valid student DTO.
            var studentDto = new CreateStudentDto
            {
                StudentId = 4,
                FullName = "Valid Student",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Gender = "Female",
                Batch = "2020",
                Address = "456 Street",
                Email = "valid@domain.com",
                PhoneNumber = "1234567890",
                StatusId = 1,
                ProgramId = 1,
                FacultyId = 1,
            };
            _context.Setup(c => c.Students.FindAsync(studentDto.StudentId)).ReturnsAsync((Student?)null);

            // Act
            var createdStudent = await _studentService.CreateNewStudent(studentDto);

            // Assert
            createdStudent.Should().NotBeNull();
            createdStudent.StudentId.Should().Be(studentDto.StudentId);
            createdStudent.FullName.Should().Be(studentDto.FullName);
            createdStudent.Email.Should().Be(studentDto.Email);
            createdStudent.PhoneNumber.Should().Be(studentDto.PhoneNumber);

        }
    }
}
