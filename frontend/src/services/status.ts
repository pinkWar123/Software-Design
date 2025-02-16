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
