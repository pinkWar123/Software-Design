import { Space, Table, Input, Select } from 'antd';
import { ColumnsType } from 'antd/es/table';
import IStudent from '../models/Student';
import React, { useEffect, useState } from 'react';
import { callDeleteStudent } from '../services/student';
import StudentCreateModal from './StudentCreateModal';
import { DeleteOutlined, EditOutlined } from '@ant-design/icons';
import StudentUpdateModal from './StudentUpdateModal';
import IProgram from '../models/Program';
import IStatus from '../models/Status';
import IFaculty from '../models/Faculty';
const { Search } = Input;

interface StudentListProps {
    students: IStudent[];
    studyPrograms: IProgram[];
    statuses: IStatus[];
    faculties: IFaculty[];
    updateStudents: () => Promise<void>;
}

const StudentList : React.FC<StudentListProps> = ({students, studyPrograms, statuses, faculties, updateStudents}) => {
    const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
    const [selectedStudent, setSelectedStudent] = useState<IStudent | null>(null);
    const [selectedFaculty, setSelectedFaculty] = useState<number | null>(null);
    const [searchText, setSearchText] = useState('');
    const [filteredStudents, setFilteredStudents] = useState<IStudent[]>([]);
    
    const handleUpdateStudent = async(student: IStudent) => {
        setSelectedStudent(student);
        setIsUpdateModalOpen(true);
    }

    const handleDeleteStudent = async(studentId: number) => {
        await callDeleteStudent(studentId);
        await updateStudents();
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
                        options={faculties.map(faculty => ({
                            label: faculty.name,
                            value: faculty.id
                        }))}
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