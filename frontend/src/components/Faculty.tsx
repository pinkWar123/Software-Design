import { useCallback, useEffect, useState } from "react";
import { Table, Button, Modal, Form, Input, message, Space, App } from "antd";
import type { ColumnsType } from "antd/es/table";
import IFaculty from "../models/Faculty";
import {
  callCreateFaculty,
  callDeleteFaculty,
  callGetFaculties,
  callUpdateFaculty,
} from "../services/faculty";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";

interface FacultyListProps {
  faculties: IFaculty[];
  updateFaculties: (faculties: IFaculty[]) => void;
}
const FacultyList: React.FC<FacultyListProps> = ({
  faculties,
  updateFaculties,
}) => {
  const fetchFaculties = useCallback(async () => {
    const response = await callGetFaculties();
    updateFaculties(response.data);
  }, []);

  useEffect(() => {
    fetchFaculties();
  }, []);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingFaculty, setEditingFaculty] = useState<IFaculty | null>(null);
  const [form] = Form.useForm();
  const { modal, message } = App.useApp();
  const handleDeleteButtonClicked = (faculty: IFaculty) => {
    modal.warning({
      title: "Xác nhận xóa",
      content: "Bạn có chắc chắn muốn xóa khoa này không?",

      closable: true,
      onOk: async () => {
        try {
          await callDeleteFaculty(faculty.id);
          await fetchFaculties();
          message.success("Xóa khoa thành công!");
        } catch (error) {
          message.error("Có lỗi xảy ra!");
        }
      },
      cancelText: "Hủy",
      okCancel: true,
      onCancel: () => {
        message.info("Đã hủy xóa khoa.");
      },
    });
  };

  const columns: ColumnsType<IFaculty> = [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
    },
    {
      title: "Tên khoa",
      dataIndex: "name",
      key: "name",
      render: (text: string) => <div style={{ cursor: "pointer" }}>{text}</div>,
    },
    {
      title: "Thao tác",
      key: "action",
      render: (_: any, record: IFaculty) => (
        <Space>
          <Button type="link" onClick={() => handleEdit(record)}>
            <EditOutlined />
          </Button>
          <Button type="link">
            <DeleteOutlined
              style={{ color: "red" }}
              onClick={() => handleDeleteButtonClicked(record)}
            />
          </Button>
        </Space>
      ),
    },
  ];

  const handleEdit = (faculty: IFaculty) => {
    setEditingFaculty(faculty);
    form.setFieldsValue(faculty);
    setIsModalOpen(true);
  };

  const handleAdd = () => {
    setEditingFaculty(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleSubmit = async (values: any) => {
    try {
      if (!editingFaculty) {
        await callCreateFaculty(values);
        await fetchFaculties();
        setIsModalOpen(false);
        message.success("Thêm khoa thành công!");
        return;
      }
      await callUpdateFaculty(editingFaculty.id, values);
      await fetchFaculties();
      setIsModalOpen(false);
      message.success("Cập nhật khoa thành công!");
    } catch (error) {
      message.error("Có lỗi xảy ra!");
    }
  };

  return (
    <div>
      <Button type="primary" onClick={handleAdd} style={{ marginBottom: 16 }}>
        Thêm khoa mới
      </Button>

      <Table columns={columns} dataSource={faculties} rowKey="id" bordered />

      <Modal
        title={editingFaculty ? "Sửa thông tin khoa" : "Thêm khoa mới"}
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
      >
        <Form form={form} onFinish={handleSubmit} layout="vertical">
          <Form.Item
            name="name"
            label="Tên khoa"
            rules={[{ required: true, message: "Vui lòng nhập tên khoa!" }]}
          >
            <Input />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit">
              {editingFaculty ? "Cập nhật" : "Thêm mới"}
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default FacultyList;
