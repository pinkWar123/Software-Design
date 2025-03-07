import { Checkbox } from "antd";
import { IStudentNotification } from "../enums/notification";

export interface StudentNotificationsProps {
}

const notificationOptions = [
    { label: 'Email', value: IStudentNotification.Email },
    { label: 'SMS', value: IStudentNotification.SMS },
    { label: 'Zalo', value: IStudentNotification.Zalo },
];

const StudentNotifications : React.FC<StudentNotificationsProps> = () => {
    return <>
        <Checkbox.Group
        onChange={values => console.log('Selected:', values)}
          options={notificationOptions}
        />
    </>
}

export default StudentNotifications;