import IProgram from "./Program";
import IStatus from "./Status";

interface IStudent {
    studentId: number;
    fullName: string;
    dateOfBirth: string;
    gender: string;
    faculty: string;
    batch: string;
    program: IProgram;
    address: string;
    email: string;
    phoneNumber: string;
    status: IStatus;
}

// - Mã số sinh viên
// - Họ tên
// - Ngày tháng năm sinh
// - Giới tính
// - Khoa 
// - Khóa 
// - Chương trình 
// - Địa chỉ
// - Email
// - Số điện thoại liên hệ
// - Tình trạng sinh viên

export default IStudent;