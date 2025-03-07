import React, { useState } from 'react';
import { Button, Modal, Form, Input, Select, DatePicker, message, App, Checkbox } from 'antd';
import { callCreateStudent } from '../services/student';
import IProgram from '../models/Program';
import IStatus from '../models/Status';
import IFaculty from '../models/Faculty';
import { ValidationError } from '../helpers/errors';
import StudentNotifications from './StudenttNotifications';
import { notificationOptions } from '../constants/notificationOptions';
interface StudentCreateModalProps {
    studyPrograms: IProgram[];
    statuses: IStatus[];
    faculties: IFaculty[];
    updateStudents: () => Promise<void>;
}

const StudentCreateModal: React.FC<StudentCreateModalProps> = ({ studyPrograms, statuses, faculties, updateStudents }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const {message} = App.useApp();
  const [form] = Form.useForm();

  const showModal = () => {
    setIsModalOpen(true);
  };

  const handleCancel = () => {
    form.resetFields();
    setIsModalOpen(false);
  };


  const handleSubmit = async (values: any) => {
    try {
      await callCreateStudent({
        ...values,
        dateOfBirth: values.dateOfBirth,
        subscribeTo: values.subscribeToNotifications
    });
      message.success('Tạo học sinh mới thành công!');
      handleCancel();
      await updateStudents();
    } catch (error) {
      console.log(error);
      const _error = error as ValidationError;
      message.error(_error?.response?.data?.title ?? "Có lỗi xảy ra khi tạo học sinh mới");
    }
  };



  return (
    <>
      <Button type="primary" onClick={showModal}>
        Tạo học sinh mới
      </Button>
      <Modal
        title="Tạo học sinh mới"
        open={isModalOpen}
        onCancel={handleCancel}
        footer={null}
      >
        <Form form={form} onFinish={handleSubmit} layout="vertical">
          <Form.Item
            name="studentId"
            label="Mã số sinh viên"
            rules={[{ required: true, message: 'Vui lòng nhập mã số sinh viên!' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="fullName"
            label="Họ và tên"
            rules={[{ required: true, message: 'Vui lòng nhập họ và tên!' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="dateOfBirth"
            label="Ngày sinh"
            rules={[{ required: true, message: 'Vui lòng chọn ngày sinh!' }]}
          >
            <DatePicker format="DD/MM/YYYY" />
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
            name="facultyId"
            label="Khoa"
            rules={[{ required: true, message: 'Vui lòng chọn khoa!' }]}
          >
            <Select>
              {faculties.length > 0 ? faculties.map(faculty => (
                <Select.Option key={faculty.id} value={faculty.id}>
                  {faculty.name}
                </Select.Option>
              )) : []}
            </Select>
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
              {studyPrograms.length > 0 ? studyPrograms.map(program => (
                <Select.Option key={program.id} value={program.id}>
                  {program.name}
                </Select.Option>
              )) : []}
            </Select>
          </Form.Item>
          <Form.Item
            name="statusId"
            label="Trạng thái"
            rules={[{ required: true, message: 'Vui lòng chọn trạng thái!' }]}
          >
            <Select>
              {statuses.length > 0 ? statuses.map(status => (
                <Select.Option key={status.id} value={status.id}>
                  {status.name}
                </Select.Option>
              )) : []}
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
              Tạo mới
            </Button>
          </Form.Item>
          <Form.Item name="subscribeToNotifications" label="Nhận thông báo qua">
            <Checkbox.Group
              options={notificationOptions} 
            />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default StudentCreateModal;