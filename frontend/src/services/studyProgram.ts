import IProgram from "../models/Program";
import axiosInstance from "./axios.config";

export const callGetStudyPrograms = async () => {
    try {
        const response = await axiosInstance.get('/StudyProgram');
        return response.data;
    } catch (error) {
        console.error('Error fetching study programs:', error);
        throw error;
    }
}

export const callCreateStudyProgram = async (studyProgram: IProgram) => {
    return await axiosInstance.post('/StudyProgram', studyProgram);
}

export const callUpdateStudyProgram = async (id: number, studyProgram: IProgram) => {
    return await axiosInstance.put(`/StudyProgram/${id}`, studyProgram);
}
