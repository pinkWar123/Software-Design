import IFaculty from "./Faculty";
import IProgram from "./Program";
import IStatus from "./Status";

interface IStudent {
    studentId: number;
    fullName: string;
    dateOfBirth: string;
    gender: string;
    facultyId: number;
    batch: string;
    programId: number;
    address: string;
    email: string;
    phoneNumber: string;
    statusId: number;
    faculty: IFaculty;
    program: IProgram;
    status: IStatus;
    createdAt: string;
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