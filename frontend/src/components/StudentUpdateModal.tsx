import { Modal, Form, Input, Select, DatePicker, Button, message } from 'antd';
import { useEffect } from 'react';
import { callUpdateStudent } from '../services/student';
import IStudent from '../models/Student';
import IProgram from '../models/Program';
import IStatus from '../models/Status';
import dayjs from 'dayjs';


interface StudentUpdateModalProps {
    student: IStudent | null;
    studyPrograms: IProgram[];
    statuses: IStatus[];
    isOpen: boolean;
    onClose: () => void;
    updateStudents: () => Promise<void>;
}

const StudentUpdateModal: React.FC<StudentUpdateModalProps> = ({
    student,
    studyPrograms,
    statuses,
    isOpen,
    onClose,
    updateStudents
}) => {
    if(!student) return <></>;
    const [form] = Form.useForm();

    useEffect(() => {
        if (student) {
            form.setFieldsValue({
                ...student,
                dateOfBirth: dayjs(student.dateOfBirth),
                programId: student.program.id,
                statusId: student.status.id
            });
        }
    }, [student, form]);

    const handleSubmit = async (values: any) => {
        try {
            const updatedStudent = await callUpdateStudent({
                ...values,
                studentId: student.studentId,
                dateOfBirth: values.dateOfBirth
            });
            console.log(updatedStudent);
            message.success('Cập nhật sinh viên thành công!');
            await updateStudents();
            onClose();
        } catch (error) {
            console.error('Error updating student:', error);
            message.error('Có lỗi xảy ra khi cập nhật sinh viên!');
        }
    };

    return (
        <Modal
            title="Cập nhật thông tin sinh viên"
            open={isOpen}
            onCancel={onClose}
            footer={null}
        >
            <Form form={form} initialValues={student} onFinish={handleSubmit} layout="vertical">
                <Form.Item
                    name="fullName"
                    label="Họ và tên"
                    rules={[{ required: true, message: 'Vui lòng nhập họ và tên!' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    name="dateOfBirth"
                    getValueProps={(value) => ({
                        value: value ? dayjs(value) : null
                    })}
                    getValueFromEvent={(date) => date?.format('YYYY-MM-DD')}
                    label="Ngày sinh"
                    rules={[{ required: true, message: 'Vui lòng chọn ngày sinh!' }]}
                >
                    <DatePicker  format="DD/MM/YYYY" />
                </Form.Item>
                <Form.Item
                    name="gender"
                    label="Giới tính"
                    rules={[{ required: true, message: 'Vui lòng chọn giới tính!' }]}
                >
                    <Select>
                        <Select.Option value="Nam">Nam</Select.Option>
                        <Select.Option value="Nữ">Nữ</Select.Option>
                        <Select.Option value="Khác">Khác</Select.Option>
                    </Select>
                </Form.Item>
                <Form.Item
                    name="faculty"
                    label="Khoa"
                    rules={[{ required: true, message: 'Vui lòng nhập khoa!' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    name="batch"
                    label="Khóa"
                    rules={[{ required: true, message: 'Vui lòng nhập khóa!' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    name="address"
                    label="Địa chỉ"
                    rules={[{ required: true, message: 'Vui lòng nhập địa chỉ!' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    name="programId"
                    label="Chương trình học"
                    rules={[{ required: true, message: 'Vui lòng chọn chương trình học!' }]}
                >
                    <Select>
                        {studyPrograms.map(program => (
                            <Select.Option key={program.id} value={program.id}>
                                {program.name}
                            </Select.Option>
                        ))}
                    </Select>
                </Form.Item>
                <Form.Item
                    name="statusId"
                    label="Trạng thái"
                    rules={[{ required: true, message: 'Vui lòng chọn trạng thái!' }]}
                >
                    <Select>
                        {statuses.map(status => (
                            <Select.Option key={status.id} value={status.id}>
                                {status.name}
                            </Select.Option>
                        ))}
                    </Select>
                </Form.Item>
                <Form.Item
                    name="email"
                    label="Email"
                    rules={[
                        { required: true, message: 'Vui lòng nhập email!' },
                        { type: 'email', message: 'Email không hợp lệ!' }
                    ]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    name="phoneNumber"
                    label="Số điện thoại"
                    rules={[{ required: true, message: 'Vui lòng nhập số điện thoại!' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item>
                    <Button type="primary" htmlType="submit">
                        Cập nhật
                    </Button>
                </Form.Item>
            </Form>
        </Modal>
    );
};

export default StudentUpdateModal; 