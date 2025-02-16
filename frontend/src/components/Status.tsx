import { useState } from 'react';
import { Table, Button, Modal, Form, Input, message } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import IStatus from "../models/Status";
import { callCreateStatus, callGetAllStatuses, callUpdateStatus } from '../services/status';

interface StatusListProps {
    statuses: IStatus[];
    updateStatuses: (statuses: IStatus[]) => void;
}

const StatusList: React.FC<StatusListProps> = ({statuses, updateStatuses}) => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingStatus, setEditingStatus] = useState<IStatus | null>(null);
    const [form] = Form.useForm();

    const fetchStatuses = async () => {
        const response = await callGetAllStatuses();
        updateStatuses(response);
    };

    const columns: ColumnsType<IStatus> = [
        {
            title: 'ID',
            dataIndex: 'id',
            key: 'id',
        },
        {
            title: 'Trạng thái',
            dataIndex: 'name',
            key: 'name',
            render: (text: string, record: IStatus) => (
                <div style={{ cursor: 'pointer' }} onClick={() => handleEdit(record)}>
                    {text}
                </div>
            )
        },
        {
            title: 'Thao tác',
            key: 'action',
            render: (_: any, record: IStatus) => (
                <Button type="link" onClick={() => handleEdit(record)}>
                    Sửa
                </Button>
            )
        }
    ];

    const handleEdit = (status: IStatus) => {
        setEditingStatus(status);
        form.setFieldsValue(status);
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
                message.success('Thêm trạng thái thành công!');
            } else {
                await callUpdateStatus(editingStatus.id, values);
                await fetchStatuses();
                message.success('Cập nhật trạng thái thành công!');
            }
            setIsModalOpen(false);
        } catch (error) {
            message.error('Có lỗi xảy ra!');
        }
    };

    return (
        <div>
            <Button 
                type="primary" 
                onClick={handleAdd} 
                style={{ marginBottom: 16 }}
            >
                Thêm trạng thái mới
            </Button>

            <Table
                columns={columns}
                dataSource={statuses}
                rowKey="id"
                bordered
            />

            <Modal
                title={editingStatus ? "Sửa thông tin trạng thái" : "Thêm trạng thái mới"}
                open={isModalOpen}
                onCancel={() => setIsModalOpen(false)}
                footer={null}
            >
                <Form
                    form={form}
                    onFinish={handleSubmit}
                    layout="vertical"
                >
                    <Form.Item
                        name="name"
                        label="Tên trạng thái"
                        rules={[{ required: true, message: 'Vui lòng nhập tên trạng thái!' }]}
                    >
                        <Input />
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit">
                            {editingStatus ? 'Cập nhật' : 'Thêm mới'}
                        </Button>
                    </Form.Item>
                </Form>
            </Modal>
        </div>
    );
};

export default StatusList;