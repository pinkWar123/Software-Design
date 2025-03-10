using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Document;

namespace backend.Services.DocumentGeneration
{
    public class MarkdownDocumentGenerator : IDocumentGenerator
    {
        public Task<byte[]> GenerateDocument(StudentDocumentDto data)
        {
            string markdownContent = $@"
    **TRƯỜNG ĐẠI HỌC {data.UniversityName}**  
    **PHÒNG ĐÀO TẠO**  
    📍 Địa chỉ: {data.Address}  
    📞 Điện thoại: {data.Phone} | 📧 Email: {data.Email}  

    ---

    ### **GIẤY XÁC NHẬN TÌNH TRẠNG SINH VIÊN**

    Trường Đại học {data.UniversityName} xác nhận:

    **1. Thông tin sinh viên:**  
    - **Họ và tên:** {data.StudentName}  
    - **Mã số sinh viên:** {data.StudentId}  
    - **Ngày sinh:** {data.Dob}  
    - **Giới tính:** {data.Gender}  
    - **Khoa:** {data.Faculty}  
    - **Chương trình đào tạo:** {data.Program}  
    - **Khóa:** {data.Course}  

    **2. Tình trạng sinh viên hiện tại:**  
    - Đang theo học  
    - Đã hoàn thành chương trình, chờ xét tốt nghiệp  
    - Đã tốt nghiệp  
    - Bảo lưu  
    - Đình chỉ học tập  
    - Tình trạng khác  

    **3. Mục đích xác nhận:**
    - {data.Reason ?? ""}  

    **4. Thời gian cấp giấy:**  
    - Giấy xác nhận có hiệu lực đến ngày: {data.ValidUntil} (tùy vào mục đích xác nhận)  

    📍 **Xác nhận của Trường Đại học {data.UniversityName}**  

    📅 Ngày cấp: {data.IssueDate}  

    🖋 **Trưởng Phòng Đào Tạo**  
    (Ký, ghi rõ họ tên, đóng dấu)
    ";
            byte[] result = System.Text.Encoding.UTF8.GetBytes(markdownContent);
            return Task.FromResult(result);
            }
    }
}