import { IStudentNotification } from "../enums/notification";

const notificationOptions = [
    { label: 'Email', value: IStudentNotification.Email },
    { label: 'SMS', value: IStudentNotification.SMS },
    { label: 'Zalo', value: IStudentNotification.Zalo },
];

export {notificationOptions}