import { Space, Table, Input } from 'antd';
import { ColumnsType } from 'antd/es/table';
import IStudent from '../models/Student';
import { useEffect, useState } from 'react';
import { callGetAllStudents,  callDeleteStudent } from '../services/student';
import StudentCreateModal from './StudentCreateModal';
import IProgram from '../models/Program';
import IStatus from '../models/Status';
import { callGetStudyPrograms } from '../services/studyProgram';
import { callGetAllStatuses } from '../services/status';
import { DeleteOutlined, EditOutlined } from '@ant-design/icons';
import StudentUpdateModal from './StudentUpdateModal';

const { Search } = Input;

const StudentList = () => {
    const [students, setStudents] = useState<IStudent[]>([]);
    const [studyPrograms, setStudyPrograms] = useState<IProgram[]>([]);
    const [statuses, setStatuses] = useState<IStatus[]>([]);
    const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
    const [selectedStudent, setSelectedStudent] = useState<IStudent | null>(null);
    const [searchText, setSearchText] = useState('');
    const [filteredStudents, setFilteredStudents] = useState<IStudent[]>([]);

    const updateStudents = async () => {
        const students = await callGetAllStudents();
        setStudents(students);
    }
    const handleUpdateStudent = async(student: IStudent) => {
        setSelectedStudent(student);
        setIsUpdateModalOpen(true);
    }

    const handleDeleteStudent = async(studentId: number) => {
        await callDeleteStudent(studentId);
        await updateStudents();
    }
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
        dataIndex: 'faculty',
        key: 'faculty', 
      },
      {
        title: 'Lớp',
        dataIndex: 'class',
        key: 'class',
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
    // const students: IStudent[] = [
    //     {
    //       id: "SV001",
    //       name: "Nguyễn Văn An",
    //       birthDate: new Date("2003-05-15"),
    //       gender: "Nam",
    //       faculty: "Công nghệ thông tin",
    //       class: "CNTT2021",
    //       program: {
    //         name: "Kỹ sư công nghệ thông tin",
    //       },
    //       address: "123 Nguyễn Văn Cừ, Quận 5, TP.HCM",
    //       email: "an.nguyenvan@student.hcmus.edu.vn",
    //       phone: "0901234567",
    //       status: {
    //         name: "Đang học"
    //       }
    //     },
    //     {
    //       id: "SV002",
    //       name: "Trần Thị Bình",
    //       birthDate: new Date("2003-08-20"),
    //       gender: "Nữ",
    //       faculty: "Kinh tế",
    //       class: "KT2021",
    //       program: {
    //         name: "Cử nhân kinh tế",
    //       },
    //       address: "456 Lê Đại Hành, Quận 11, TP.HCM",
    //       email: "binh.tranthi@student.hcmus.edu.vn",
    //       phone: "0912345678",
    //       status: {
    //         name: "Đang học"
    //       }
    //     },
    //     {
    //       id: "SV003",
    //       name: "Lê Hoàng Cường",
    //       birthDate: new Date("2002-12-10"),
    //       gender: "Nam",
    //       faculty: "Vật lý",
    //       class: "VL2020",
    //       program: {
    //         name: "Cử nhân vật lý"
    //       },
    //       address: "789 Lý Thường Kiệt, Quận 10, TP.HCM",
    //       email: "cuong.lehoang@student.hcmus.edu.vn",
    //       phone: "0923456789",
    //       status: {
    //         name: "Đang học"
    //       }
    //     }
    //   ];

    useEffect(() => {
        const fetchStudents = async () => {
            try {
                const response = await callGetAllStudents();
                setStudents(response);
            } catch (error) {
                console.error('Error fetching students:', error);
            }
        };
        fetchStudents();
    }, []);

    useEffect(() => {
        const fetchStudyPrograms = async () => {
            try {
                const response = await callGetStudyPrograms();
                setStudyPrograms(response);
            } catch (error) {
                console.error('Error fetching study programs:', error);
            }
        };
        fetchStudyPrograms();
    }, []);

    useEffect(() => {
        const fetchStatuses = async () => {
            try {
                const response = await callGetAllStatuses();
                setStatuses(response);
            } catch (error) {
                console.error('Error fetching statuses:', error);
            }
        };
        fetchStatuses();    
    }, [])

    useEffect(() => {
        setFilteredStudents(students);
    }, [students]);

    const handleSearch = (value: string) => {
        setSearchText(value);
        const filtered = students.filter((student) => {
            const searchValue = value.toLowerCase();
            return (
                student.fullName.toLowerCase().includes(searchValue) ||
                student.studentId.toString().toLowerCase().includes(searchValue)
            );
        });
        setFilteredStudents(filtered);
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
                <StudentCreateModal studyPrograms={studyPrograms} statuses={statuses} updateStudents={updateStudents} />
                {selectedStudent && (
                    <StudentUpdateModal 
                        student={selectedStudent}
                        studyPrograms={studyPrograms}
                        statuses={statuses}
                        isOpen={isUpdateModalOpen}
                        onClose={() => {
                            setIsUpdateModalOpen(false);
                            setSelectedStudent(null);
                        }}
                        updateStudents={updateStudents}
                    />
                )}
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