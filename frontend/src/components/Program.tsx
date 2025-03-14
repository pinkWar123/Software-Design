import { useState } from "react";
import { Table, Button, Modal, Form, Input, message, Space, App } from "antd";
import type { ColumnsType } from "antd/es/table";
import IProgram from "../models/Program";
import {
  callCreateStudyProgram,
  callDeleteStudyProgram,
  callGetStudyPrograms,
  callUpdateStudyProgram,
} from "../services/studyProgram";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";

interface ProgramListProps {
  programs: IProgram[];
  updatePrograms: (programs: IProgram[]) => void;
}

const ProgramList: React.FC<ProgramListProps> = ({
  programs,
  updatePrograms,
}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProgram, setEditingProgram] = useState<IProgram | null>(null);
  const [form] = Form.useForm();
  const { message, modal } = App.useApp();
  const handleDeleteButtonClicked = (program: IProgram) => {
    modal.warning({
      title: "Xác nhận xóa",
      content: "Bạn có chắc chắn muốn xóa chương trình này không?",
      onOk: async () => {
        try {
          await callDeleteStudyProgram(program.id);
          await fetchPrograms();
          message.success("Xóa chương trình thành công!");
        } catch (error) {
          message.error("Có lỗi xảy ra!");
        }
      },
      closable: true,
      cancelText: "Hủy",
      okCancel: true,
    });
  };
  const fetchPrograms = async () => {
    const response = await callGetStudyPrograms();
    updatePrograms(response);
  };

  const columns: ColumnsType<IProgram> = [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
    },
    {
      title: "Tên chương trình",
      dataIndex: "name",
      key: "name",
      render: (text: string, record: IProgram) => (
        <div style={{ cursor: "pointer" }} onClick={() => handleEdit(record)}>
          {text}
        </div>
      ),
    },
    {
      title: "Thao tác",
      key: "action",
      render: (_: any, record: IProgram) => (
        <Space>
          <Button type="link" onClick={() => handleEdit(record)}>
            <EditOutlined />
          </Button>
          <Button type="link" onClick={() => handleDeleteButtonClicked(record)}>
            <DeleteOutlined style={{ color: "red" }} />
          </Button>
        </Space>
      ),
    },
  ];

  const handleEdit = (program: IProgram) => {
    setEditingProgram(program);
    form.setFieldsValue(program);
    setIsModalOpen(true);
  };

  const handleAdd = () => {
    setEditingProgram(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleSubmit = async (values: any) => {
    try {
      if (!editingProgram) {
        await callCreateStudyProgram(values);
        await fetchPrograms();
        message.success("Thêm chương trình thành công!");
      } else {
        await callUpdateStudyProgram(editingProgram.id, values);
        await fetchPrograms();
        message.success("Cập nhật chương trình thành công!");
      }
      setIsModalOpen(false);
    } catch (error) {
      message.error("Có lỗi xảy ra!");
    }
  };

  return (
    <div>
      <Button type="primary" onClick={handleAdd} style={{ marginBottom: 16 }}>
        Thêm chương trình mới
      </Button>

      <Table columns={columns} dataSource={programs} rowKey="id" bordered />

      <Modal
        title={
          editingProgram
            ? "Sửa thông tin chương trình"
            : "Thêm chương trình mới"
        }
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
      >
        <Form form={form} onFinish={handleSubmit} layout="vertical">
          <Form.Item
            name="name"
            label="Tên chương trình"
            rules={[
              { required: true, message: "Vui lòng nhập tên chương trình!" },
            ]}
          >
            <Input />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit">
              {editingProgram ? "Cập nhật" : "Thêm mới"}
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default ProgramList;
