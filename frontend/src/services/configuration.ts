import { IConfiguration } from "../models/Configuration"
import axiosInstance from "./axios.config"

export const callGetAllConfigurations = async() => {
    return (await axiosInstance.get<IConfiguration[]>('/Configuration')).data
}

export const callActivateConfiguration = async(key: string) => {
    return( await axiosInstance.put<IConfiguration>(`/Configuration/${key}/activate`)).data
}

export const callDeactivateConfiguration = async(key: string) => {
    return (await axiosInstance.put<IConfiguration>(`/Configuration/${key}/deactivate`)).data
}

export const callChangeConfigurationValue = async(key: string, value: string) => {
    return (await axiosInstance.put<IConfiguration>(`/Configuration/${key}/value`, {value})).data;
}