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

