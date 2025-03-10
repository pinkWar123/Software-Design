import { DocumentType } from "../enums/document";
import axiosInstance from "./axios.config";

export const callDownloadReport = async (
  studentId: number,
  format: DocumentType,
  reason: string,
  date: string
) => {
  const response = await axiosInstance.get(
    `/Document?studentId=${studentId}&format=${format}&reason=${reason}&expiredDate=${date}`,
    {
      responseType: "blob",
    }
  );
  return response;
};
