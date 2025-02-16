import React, { useState } from 'react';
import { Button, Modal, Form, Input, Select, DatePicker, message } from 'antd';
import { callCreateStudent } from '../services/student';
import IProgram from '../models/Program';
import IStatus from '../models/Status';

interface StudentCreateModalProps {
    studyPrograms: IProgram[];
    statuses: IStatus[];
    updateStudents: () => Promise<void>;
}

const StudentCreateModal: React.FC<StudentCreateModalProps> = ({ studyPrograms, statuses, updateStudents }) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
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
      await callCreateStudent(values);
      message.success('Tạo học sinh mới thành công!');
      handleCancel();
      await updateStudents();
    } catch (error) {
      message.error('Có lỗi xảy ra khi tạo học sinh mới');
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
              Tạo mới
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default StudentCreateModal;