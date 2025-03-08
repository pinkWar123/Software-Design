import IFaculty from "../models/Faculty";
import axiosInstance from "./axios.config";

export const callGetFaculties = async () => {
  return await axiosInstance.get<IFaculty[]>("/Faculty");
};

export const callCreateFaculty = async (faculty: IFaculty) => {
  return await axiosInstance.post<IFaculty>("/Faculty", faculty);
};

export const callUpdateFaculty = async (id: number, faculty: IFaculty) => {
  return await axiosInstance.put<IFaculty>(`/Faculty/${id}`, faculty);
};

export const callDeleteFaculty = async (id: number) => {
  return await axiosInstance.delete(`/Faculty/${id}`);
};
