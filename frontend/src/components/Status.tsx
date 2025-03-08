import { useState } from "react";
import { Table, Button, Modal, Form, Input, Select, Space, App } from "antd";
import type { ColumnsType } from "antd/es/table";
import IStatus from "../models/Status";
import {
  callCreateStatus,
  callDeleteStatus,
  callGetAllStatuses,
  callUpdateStatus,
} from "../services/status";
import { DeleteOutlined, EditOutlined } from "@ant-design/icons";

interface StatusListProps {
  statuses: IStatus[];
  updateStatuses: (statuses: IStatus[]) => void;
}

const StatusList: React.FC<StatusListProps> = ({
  statuses,
  updateStatuses,
}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingStatus, setEditingStatus] = useState<IStatus | null>(null);
  const [form] = Form.useForm();
  const { modal, message } = App.useApp();
  console.log(statuses);
  const fetchStatuses = async () => {
    const response = await callGetAllStatuses();
    updateStatuses(response);
  };

  const handleDeleteButtonClicked = (status: IStatus) => {
    modal.warning({
      title: "Xác nhận xóa",
      content: "Bạn có chắc chắn muốn xóa trạng thái này không?",
      onOk: async () => {
        try {
          await callDeleteStatus(status.id);
          await fetchStatuses();
        } catch (error) {
          message.error("Có lỗi xảy ra!");
        }
      },
    });
  };

  const columns: ColumnsType<IStatus> = [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
    },
    {
      title: "Trạng thái",
      dataIndex: "name",
      key: "name",
      render: (text: string, record: IStatus) => (
        <>
          <div style={{ cursor: "pointer" }} onClick={() => handleEdit(record)}>
            {text}
          </div>
        </>
      ),
    },
    {
      title: "Thao tác",
      key: "action",
      render: (_: any, record: IStatus) => (
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

  const handleEdit = (status: IStatus) => {
    setEditingStatus(status);
    form.setFieldsValue({
      ...status,
      outgoingTransitions: status.outgoingTransitions.map(
        (transition) => transition.targetStatusId
      ),
    });
    setIsModalOpen(true);
  };

  const handleAdd = () => {
    setEditingStatus(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleSubmit = async (values: any) => {
    try {
      if (!editingStatus) {
        await callCreateStatus(values);
        await fetchStatuses();
        message.success("Thêm trạng thái thành công!");
      } else {
        await callUpdateStatus(editingStatus.id, values);
        await fetchStatuses();
        message.success("Cập nhật trạng thái thành công!");
      }
      setIsModalOpen(false);
    } catch (error) {
      message.error("Có lỗi xảy ra!");
    }
  };

  const getFilteredStatuses = () => {
    if (editingStatus) {
      return statuses.filter((status) => status.id !== editingStatus.id);
    }
    return statuses;
  };

  return (
    <div>
      <Button type="primary" onClick={handleAdd} style={{ marginBottom: 16 }}>
        Thêm trạng thái mới
      </Button>

      <Table columns={columns} dataSource={statuses} rowKey="id" bordered />

      <Modal
        title={
          editingStatus ? "Sửa thông tin trạng thái" : "Thêm trạng thái mới"
        }
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
      >
        <Form form={form} onFinish={handleSubmit} layout="vertical">
          <Form.Item
            name="name"
            label="Tên trạng thái"
            rules={[
              { required: true, message: "Vui lòng nhập tên trạng thái!" },
            ]}
          >
            <Input />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit">
              {editingStatus ? "Cập nhật" : "Thêm mới"}
            </Button>
          </Form.Item>
          {editingStatus && (
            <Form.Item name="outgoingTransitions">
              <Select
                mode="tags"
                style={{ width: "100%" }}
                placeholder="Tags Mode"
                onChange={(value) => console.log(value)}
                options={getFilteredStatuses().map((status) => ({
                  label: status.name,
                  value: status.id,
                }))}
              />
            </Form.Item>
          )}
        </Form>
      </Modal>
    </div>
  );
};

export default StatusList;
