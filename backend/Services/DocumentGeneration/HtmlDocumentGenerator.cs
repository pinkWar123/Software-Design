using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Document;

namespace backend.Services.DocumentGeneration
{
    public class HtmlDocumentGenerator : IDocumentGenerator
    {
        public Task<byte[]> GenerateDocument(StudentDocumentDto data)
    {
            string htmlContent = $@"
    <!DOCTYPE html>
    <html>
    <head>
    <meta charset=""UTF-8"">
    <title>Giấy Xác Nhận Tình Trạng Sinh Viên</title>
    <style>
        /* Add your CSS here */
    </style>
    </head>
    <body>
    <h1>TRƯỜNG ĐẠI HỌC {data.UniversityName}</h1>
    <h2>PHÒNG ĐÀO TẠO</h2>
    <p>
        📍 Địa chỉ: {data.Address}<br>
        📞 Điện thoại: {data.Phone} | 📧 Email: {data.Email}
    </p>
    <hr>
    <h3>GIẤY XÁC NHẬN TÌNH TRẠNG SINH VIÊN</h3>
    <p>Trường Đại học {data.UniversityName} xác nhận:</p>
    
    <h4>1. Thông tin sinh viên:</h4>
    <ul>
        <li><strong>Họ và tên:</strong> {data.StudentName}</li>
        <li><strong>Mã số sinh viên:</strong> {data.StudentId}</li>
        <li><strong>Ngày sinh:</strong> {data.Dob}</li>
        <li><strong>Giới tính:</strong> {data.Gender}</li>
        <li><strong>Khoa:</strong> {data.Faculty}</li>
        <li><strong>Chương trình đào tạo:</strong> {data.Program}</li>
        <li><strong>Khóa:</strong> {data.Course}</li>
    </ul>
    
    <h4>2. Tình trạng sinh viên hiện tại:</h4>
    <ul>
        <li>{data.Status}</li>
    </ul>
    
    <h4>3. Mục đích xác nhận:</h4>
    <ul>
        <li>{data.Reason ?? ""}</li>
    </ul>
    
    <h4>4. Thời gian cấp giấy:</h4>
    <p>Giấy xác nhận có hiệu lực đến ngày: {data.ValidUntil} (tùy vào mục đích xác nhận)</p>
    
    <p>
        📍 <strong>Xác nhận của Trường Đại học {data.UniversityName}</strong>
    </p>
    
    <p>📅 Ngày cấp: {data.IssueDate}</p>
    <p>🖋 <strong>Trưởng Phòng Đào Tạo</strong><br>
        (Ký, ghi rõ họ tên, đóng dấu)
    </p>
    </body>
    </html>";

            // Return the HTML as bytes using UTF8 encoding.
            byte[] result = System.Text.Encoding.UTF8.GetBytes(htmlContent);
            return Task.FromResult(result);
        }
    }
}