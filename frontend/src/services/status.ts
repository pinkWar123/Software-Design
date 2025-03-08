import IStatus from "../models/Status";
import axiosInstance from "./axios.config";

export const callGetAllStatuses = async () => {
  try {
    const response = await axiosInstance.get("/Status");
    return response.data;
  } catch (error) {
    console.error("Error fetching statuses:", error);
    throw error;
  }
};

export const callCreateStatus = async (status: IStatus) => {
  return await axiosInstance.post("/Status", status);
};

export interface ICreateStatus {
  id: number;
  name: string;
  outgoingTransitions: number[];
}

export interface IUpdateStatus extends ICreateStatus {}

export const callUpdateStatus = async (id: number, status: IUpdateStatus) => {
  return await axiosInstance.put(`/Status/${id}`, status);
};

export const callDeleteStatus = async (id: number) => {
  return (await axiosInstance.delete(`/Status/${id}`)).data;
};
