import IStudent from "../models/Student";
import axiosInstance from "./axios.config";

// ... existing code ...

export interface CreateStudentDto {
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
}

// ... existing code ...
export interface UpdateStudentDto extends CreateStudentDto {
    studentId: number;
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

export const callCreateStudent = async (student: CreateStudentDto) => {
    try {
        const response = await axiosInstance.post('/Student', student);
        return response.data;
    } catch (error) {
        console.error('Error creating student:', error);
        throw error;
    }
}

export const callUpdateStudent = async(newStudent: UpdateStudentDto) => {
    try {
        const response = await axiosInstance.put(`/Student`, newStudent);
        return response.data;
    } catch (error) {
        console.error('Error updating student:', error);
        throw error;
    }
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


