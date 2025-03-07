import { Space, Table, Input, Select, Upload, Button, App, Checkbox } from 'antd';
import { ColumnsType } from 'antd/es/table';
import IStudent from '../models/Student';
import React, { useEffect, useState } from 'react';
import { callDeleteStudent } from '../services/student';
import StudentCreateModal from './StudentCreateModal';
import { DeleteOutlined, EditOutlined, UploadOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import StudentUpdateModal from './StudentUpdateModal';
import IProgram from '../models/Program';
import IStatus from '../models/Status';
import IFaculty from '../models/Faculty';
import type { UploadProps } from 'antd';
import { IStudentNotification } from '../enums/notification';
const { Search } = Input;

interface StudentListProps {
    students: IStudent[];
    studyPrograms: IProgram[];
    statuses: IStatus[];
    faculties: IFaculty[];
    updateStudents: () => Promise<void>;
}

const notificationOptions = [
    { label: 'Email', value: IStudentNotification.Email },
    { label: 'SMS', value: IStudentNotification.SMS },
    { label: 'Zalo', value: IStudentNotification.Zalo },
  ];

const StudentList : React.FC<StudentListProps> = ({students, studyPrograms, statuses, faculties, updateStudents}) => {
    const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
    const [selectedStudent, setSelectedStudent] = useState<IStudent | null>(null);
    const [selectedFaculty, setSelectedFaculty] = useState<number | null>(null);
    const [searchText, setSearchText] = useState('');
    const [filteredStudents, setFilteredStudents] = useState<IStudent[]>([]);
    const {modal, message} = App.useApp()
    const handleUpdateStudent = async(student: IStudent) => {
        setSelectedStudent(student);
        setIsUpdateModalOpen(true);
    }

    const handleDeleteStudent = async(studentId: number) => {
        modal.confirm({
            title: 'Xác nhận xóa',
            icon: <ExclamationCircleOutlined />,
            content: 'Bạn có chắc chắn muốn xóa sinh viên này?',
            okText: 'Xóa',
            okType: 'danger',
            cancelText: 'Hủy',
            async onOk() {
                try {
                    await callDeleteStudent(studentId);
                    await updateStudents();
                    message.success('Xóa sinh viên thành công');
                } catch (error) {
                    message.error('Xóa sinh viên thất bại');
                }
            },
        });
    }

    const handleFacultyChange = (value: number | null) => {
        setSelectedFaculty(value);
        filterStudents(searchText, value);
    };

    const filterStudents = (searchValue: string, facultyId: number | null) => {
        const filtered = students.filter((student) => {
            const matchesSearch = 
                student.fullName.toLowerCase().includes(searchValue.toLowerCase()) ||
                student.studentId.toString().toLowerCase().includes(searchValue.toLowerCase());
            
            const matchesFaculty = facultyId ? student.facultyId === facultyId : true;

            return matchesSearch && matchesFaculty;
        });
        setFilteredStudents(filtered);
    };
    const renderNotificationOptions = (value: IStudentNotification[]) => (
        <Checkbox.Group
          options={notificationOptions}
          value={value}
        />
      );
    const columns: ColumnsType<IStudent> = [
      {
        title: 'Mã sinh viên',
        dataIndex: 'studentId',
        key: 'id',
      },
      {
        title: 'Họ và tên',
        dataIndex: 'fullName',
        key: 'fullName',
      },
      {
        title: 'Ngày sinh',
        dataIndex: 'dateOfBirth',
        key: 'dateOfBirth',
        render: (dateString: string) => {
            if (!dateString) return '';
            const date = new Date(dateString);
            return date.toLocaleDateString('vi-VN');
        }
      },
      {
        title: 'Giới tính',
        dataIndex: 'gender',
        key: 'gender',
      },
      {
        title: 'Khoa',
        dataIndex: ['faculty', 'name'],
        key: 'faculty', 
      },
      {
        title: 'Chương trình',
        dataIndex: ['program', 'name'],
        key: 'program',
      },
      {
        title: 'Email',
        dataIndex: 'email',
        key: 'email',
      },
      {
        title: 'Số điện thoại', 
        dataIndex: 'phoneNumber',
        key: 'phone',
      },
      {
        title: 'Trạng thái',
        dataIndex: ['status', 'name'],
        key: 'status',
      },
      {
        title: 'Hình thức nhận thông báo',
        dataIndex: 'subscribeToNotifications',
        key: 'notification',
        render: (value: IStudentNotification[]) => 
            renderNotificationOptions(value)
      },
      {
        title: 'Thao tác',
        key: 'action',
        render: (_, record) => (
            <Space size="middle">
                <EditOutlined 
                    style={{ color: '#1890ff', cursor: 'pointer' }}
                    onClick={() => handleUpdateStudent(record)}
                />
                <DeleteOutlined 
                    style={{ color: '#ff4d4f', cursor: 'pointer' }}
                    onClick={() => handleDeleteStudent(record.studentId)}    
                />
            </Space>
        ),
    }
    ];
    

    useEffect(() => {
        setFilteredStudents(students);
    }, [students]);

    const handleSearch = (value: string) => {
        setSearchText(value);
        const filtered = students.filter((student) => {
            const searchValue = value.toLowerCase();
            return (
                (student.fullName.toLowerCase().includes(searchValue) ||
                student.studentId.toString().toLowerCase().includes(searchValue))
                && (selectedFaculty == null || student.facultyId == selectedFaculty)
            );
        });
        setFilteredStudents(filtered);
    };

    const uploadProps: UploadProps = {
        name: 'file',
        accept: '.csv,.json',
        action: 'http://localhost:5215/api/Student/import',
        showUploadList: false,
        onChange(info) {
            if (info.file.status === 'done') {
                message.success('Import sinh viên thành công');
                updateStudents();
            } else if (info.file.status === 'error') {
                message.error('Import sinh viên thất bại');
            }
        },
    };

    const downloadTemplate = (type: 'csv' | 'json') => {
        let content: string;
        let filename: string;
        let mimeType: string;

        if (type === 'csv') {
            content = `FullName,DateOfBirth,Gender,Batch,Address,Email,PhoneNumber
Nguyễn Văn A,2000-01-15,Male,K15,123 Đường ABC,nguyenvana@example.com,0123456789`;
            filename = 'student_template.csv';
            mimeType = 'text/csv;charset=utf-8;';
        } else {
            content = JSON.stringify({
                students: [{
                    fullName: "Nguyễn Văn A",
                    dateOfBirth: "2000-01-15",
                    gender: "Male",
                    batch: "K15",
                    address: "123 Đường ABC",
                    email: "nguyenvana@example.com",
                    phoneNumber: "0123456789"
                }]
            }, null, 2);
            filename = 'student_template.json';
            mimeType = 'application/json';
        }

        const blob = new Blob([content], { type: mimeType });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
    };

    const handleExport = async (format: 'csv' | 'json') => {
        try {
            const response = await fetch(`http://localhost:5215/api/Student/export?format=${format}`);
            if (!response.ok) {
                throw new Error('Export failed');
            }
            
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `students.${format}`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            
            message.success(`Export to ${format.toUpperCase()} thành công`);
        } catch (error) {
            message.error('Export thất bại');
            console.error('Export error:', error);
        }
    };

    return (
        <>
            <Space direction="vertical" style={{ width: '100%', marginBottom: 16 }}>
                <Search
                    placeholder="Tìm kiếm theo tên hoặc mã sinh viên..."
                    allowClear
                    enterButton="Tìm kiếm"
                    size="large"
                    onChange={(e) => handleSearch(e.target.value)}
                    style={{ maxWidth: 500 }}
                />
                <Select
                        placeholder="Chọn khoa"
                        allowClear
                        style={{ width: 200 }}
                        onChange={handleFacultyChange}
                        options={faculties.length > 0 ? faculties.map(faculty => ({
                            label: faculty.name,
                            value: faculty.id
                        })) : []}
                    />
                <StudentCreateModal studyPrograms={studyPrograms} statuses={statuses} faculties={faculties} updateStudents={updateStudents} />
                {selectedStudent && (
                    <StudentUpdateModal 
                        student={selectedStudent}
                        studyPrograms={studyPrograms}
                        statuses={statuses}
                        faculties={faculties}
                        isOpen={isUpdateModalOpen}
                        onClose={() => {
                            setIsUpdateModalOpen(false);
                            setSelectedStudent(null);
                        }}
                        updateStudents={updateStudents}
                    />
                )}
                <Space>
                    <Upload {...uploadProps}>
                        <Button icon={<UploadOutlined />}>Import từ CSV/JSON</Button>
                    </Upload>
                    <Button onClick={() => downloadTemplate('csv')}>
                        Tải template CSV
                    </Button>
                    <Button onClick={() => downloadTemplate('json')}>
                        Tải template JSON
                    </Button>
                    <Button onClick={() => handleExport('csv')}>
                        Export to CSV
                    </Button>
                    <Button onClick={() => handleExport('json')}>
                        Export to JSON
                    </Button>
                </Space>
                <Table 
                    columns={columns} 
                    dataSource={filteredStudents}
                    rowKey="studentId" 
                />
            </Space>
        </>
    );
}

export default StudentList;