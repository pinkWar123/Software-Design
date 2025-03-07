import { IStudentNotification } from "../enums/notification";
import IStudent from "../models/Student";
import axiosInstance from "./axios.config";

// ... existing code ...

export interface CreateStudentDto {
    studentId: number;
    fullName: string;      // Họ tên
    dateOfBirth: Date;     // Ngày tháng năm sinh
    gender: string;        // Giới tính
    faculty: string;       // Khoa
    batch: string;         // Khóa
    address: string;       // Địa chỉ
    email: string;         // Email
    phoneNumber: string;   // Số điện thoại liên hệ
    statusId: number;
    programId: number;
    subscribeTo: IStudentNotification[];
}

// ... existing code ...
export interface UpdateStudentDto extends CreateStudentDto {
}

export const callGetAllStudents = async () => {
    try {
        const response = await axiosInstance.get<IStudent[]>('/Student')   ;
        return response.data;
    } catch (error) {
        console.error('Error fetching students:', error);
        throw error;
    }
}

// studentService.ts
export const callCreateStudent = async (student: CreateStudentDto) => {
    // No need for try-catch here since the interceptor handles errors.
    const response = await axiosInstance.post('/Student', student);
    return response.data;
  }
  

export const callUpdateStudent = async(studentId: number, newStudent: UpdateStudentDto) => {
   
        const response = await axiosInstance.put(`/Student/${studentId}`, newStudent);
        return response.data;
    
}

export const callDeleteStudent = async(studentId: number) => {
    try {
        const response = await axiosInstance.delete(`/Student/${studentId}`);
        return response.data;
    } catch (error) {
        console.error('Error deleting student:', error);
        throw error;
    }
}


