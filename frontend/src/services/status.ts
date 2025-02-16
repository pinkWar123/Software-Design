import IStatus from "../models/Status";
import axiosInstance from "./axios.config";

export const callGetAllStatuses = async () => {
    try {   
        const response = await axiosInstance.get('/Status');
        return response.data;
    } catch (error) {
        console.error('Error fetching statuses:', error);
        throw error;
    }
}

export const callCreateStatus = async (status: IStatus) => {
    return await axiosInstance.post('/Status', status);
}

export const callUpdateStatus = async (id: number, status: IStatus) => {
    return await axiosInstance.put(`/Status/${id}`, status);
}

