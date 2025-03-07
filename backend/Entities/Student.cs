using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Entities
{
    public class Student
    {
        public int StudentId { get; set; }  // Mã số sinh viên
        public string FullName { get; set; }   // Họ tên
        public DateTime DateOfBirth { get; set; }  // Ngày tháng năm sinh 
        public string Gender { get; set; }     // Giới tính
        public string Batch { get; set; }      // Khóa
        public string Address { get; set; }    // Địa chỉ
        public string Email { get; set; }      // Email
        public string PhoneNumber { get; set; }  // Số điện thoại liên hệ
        public int StatusId { get; set; }
        public int ProgramId { get; set; }
        public int FacultyId { get; set; }
        
        // Navigation properties
        public Status Status { get; set; }     // Tình trạng sinh viên
        public StudyProgram Program { get; set; }    // Chương trình
        public Faculty Faculty { get; set; }
        public List<StudentNotification> SubscribeToNotifications { get; set; } = new List<StudentNotification>();
        public DateTime CreatedAt { get; set; }
    }
}

