using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Student
{
    public class StudentImportDto
    {
        public string FullName { get; set; }   // Họ tên
        public DateTime DateOfBirth { get; set; }  // Ngày tháng năm sinh 
        public string Gender { get; set; }     // Giới tính
        public string Batch { get; set; }      // Khóa
        public string Address { get; set; }    // Địa chỉ
        public string Email { get; set; }      // Email
        public string PhoneNumber { get; set; }  // Số điện thoại liên hệ
    }
}