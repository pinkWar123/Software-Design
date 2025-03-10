import {
  Space,
  Table,
  Input,
  Select,
  Upload,
  Button,
  App,
  Checkbox,
  Menu,
  Dropdown,
  Modal,
  Radio,
  DatePicker,
} from "antd";
import { ColumnsType } from "antd/es/table";
import IStudent from "../models/Student";
import React, { useEffect, useState } from "react";
import { callDeleteStudent } from "../services/student";
import StudentCreateModal from "./StudentCreateModal";
import {
  DeleteOutlined,
  EditOutlined,
  UploadOutlined,
  ExclamationCircleOutlined,
  DownloadOutlined,
} from "@ant-design/icons";
import StudentUpdateModal from "./StudentUpdateModal";
import IProgram from "../models/Program";
import IStatus from "../models/Status";
import IFaculty from "../models/Faculty";
import type { UploadProps } from "antd";
import { IStudentNotification } from "../enums/notification";
import { callDownloadReport } from "../services/document";
import { DocumentType } from "../enums/document";
const { Search } = Input;
import { saveAs } from "file-saver";
import moment, { Moment } from "moment";
interface StudentListProps {
  students: IStudent[];
  studyPrograms: IProgram[];
  statuses: IStatus[];
  faculties: IFaculty[];
  updateStudents: () => Promise<void>;
}
const entries = Object.entries(DocumentType).filter(([key, value]) =>
  isNaN(Number(key))
);
const notificationOptions = [
  { label: "Email", value: IStudentNotification.Email },
  { label: "SMS", value: IStudentNotification.SMS },
  { label: "Zalo", value: IStudentNotification.Zalo },
];
const reasons = [
  "Xác nhận đang học để vay vốn ngân hàng",
  "Xác nhận làm thủ tục tạm hoãn nghĩa vụ quân sự",
  "Xác nhận làm hồ sơ xin việc / thực tập",
  "Xác nhận lý do khác",
];
const reasonValidityMap: { [key: string]: number } = {
  "Xác nhận đang học để vay vốn ngân hàng": 90,
  "Xác nhận làm thủ tục tạm hoãn nghĩa vụ quân sự": 180,
  "Xác nhận làm hồ sơ xin việc / thực tập": 60,
};
const StudentList: React.FC<StudentListProps> = ({
  students,
  studyPrograms,
  statuses,
  faculties,
  updateStudents,
}) => {
  const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
  const [selectedStudent, setSelectedStudent] = useState<IStudent | null>(null);
  const [selectedFaculty, setSelectedFaculty] = useState<number | null>(null);
  const [searchText, setSearchText] = useState("");
  const [filteredStudents, setFilteredStudents] = useState<IStudent[]>([]);
  const { modal, message } = App.useApp();
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [selectedFormat, setSelectedFormat] = useState<DocumentType | null>(
    null
  );
  const [selectedReason, setSelectedReason] = useState<string>("");
  const [customReason, setCustomReason] = useState<string>("");
  const [expiryDate, setExpiryDate] = useState<Moment | null>(null);
  const [customExpiryDate, setCustomExpiryDate] = useState<Moment | null>(null);
  // Open modal when user selects a format from the dropdown menu
  useEffect(() => {
    if (selectedReason && selectedReason !== "Xác nhận lý do khác") {
      const daysToAdd = reasonValidityMap[selectedReason] || 0;
      setExpiryDate(moment().add(daysToAdd, "days"));
    } else {
      // Reset expiry date for custom reason so user can pick one
      setExpiryDate(null);
    }
  }, [selectedReason]);
  const showDownloadModal = (student: IStudent, format: DocumentType) => {
    setSelectedStudent(student);
    setSelectedFormat(format);
    setSelectedReason(""); // reset previous reason
    setCustomReason("");
    setIsModalVisible(true);
    setCustomExpiryDate(null);
    setIsModalVisible(true);
  };

  // Handle confirmation in the modal
  const handleModalOk = async () => {
    // Determine the final reason: if "Xác nhận lý do khác" is selected, use the custom input
    const reason =
      selectedReason === "Xác nhận lý do khác" ? customReason : selectedReason;
    // Determine final expiry date:
    // - For custom reason, use customExpiryDate if provided, otherwise error out.
    // - For predefined reasons, use calculated expiryDate.
    let finalExpiryDate: Moment | null = null;
    if (selectedReason === "Xác nhận lý do khác") {
      if (!customExpiryDate) {
        message.error("Vui lòng chọn thời gian hiệu lực cho giấy xác nhận!");
        return;
      }
      finalExpiryDate = customExpiryDate;
    } else {
      finalExpiryDate = expiryDate;
    }
    if (!reason) {
      message.error("Vui lòng chọn hoặc nhập lý do xác nhận!");
      return;
    }
    if (!finalExpiryDate) {
      message.error("Thời gian hiệu lực không hợp lệ!");
      return;
    }
    try {
      // Call your API function with studentId, selectedFormat, reason, and expiry date
      // Ensure your backend API accepts these additional parameters
      await handleDownloadReport(
        selectedStudent!.studentId,
        selectedFormat!,
        reason,
        finalExpiryDate.format("YYYY-MM-DD")
      );
      setIsModalVisible(false);
    } catch (error) {
      console.error("Download error:", error);
      message.error("Có lỗi khi tải báo cáo.");
    }
  };

  const handleModalCancel = () => {
    setIsModalVisible(false);
  };
  const handleUpdateStudent = async (student: IStudent) => {
    setSelectedStudent(student);
    setIsUpdateModalOpen(true);
  };

  const handleDeleteStudent = async (studentId: number) => {
    modal.confirm({
      title: "Xác nhận xóa",
      icon: <ExclamationCircleOutlined />,
      content: "Bạn có chắc chắn muốn xóa sinh viên này?",
      okText: "Xóa",
      okType: "danger",
      cancelText: "Hủy",
      async onOk() {
        try {
          await callDeleteStudent(studentId);
          await updateStudents();
          message.success("Xóa sinh viên thành công");
        } catch (error) {
          message.error("Xóa sinh viên thất bại");
        }
      },
    });
  };

  const handleFacultyChange = (value: number | null) => {
    setSelectedFaculty(value);
    filterStudents(searchText, value);
  };

  const filterStudents = (searchValue: string, facultyId: number | null) => {
    const filtered = students.filter((student) => {
      const matchesSearch =
        student.fullName.toLowerCase().includes(searchValue.toLowerCase()) ||
        student.studentId
          .toString()
          .toLowerCase()
          .includes(searchValue.toLowerCase());

      const matchesFaculty = facultyId ? student.facultyId === facultyId : true;

      return matchesSearch && matchesFaculty;
    });
    setFilteredStudents(filtered);
  };
  const renderNotificationOptions = (value: IStudentNotification[]) => (
    <Checkbox.Group options={notificationOptions} value={value} />
  );

  const handleDownloadReport = async (
    studentId: number,
    format: DocumentType,
    reason: string,
    date: string
  ) => {
    try {
      const response = await callDownloadReport(
        studentId,
        format,
        reason,
        date
      );
      // Get file name from Content-Disposition or fallback to a default
      const disposition = response.headers["content-disposition"];
      let fileName = "report.pdf";
      if (disposition && disposition.indexOf("filename=") !== -1) {
        const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
        const matches = filenameRegex.exec(disposition);
        if (matches && matches[1]) {
          fileName = matches[1].replace(/['"]/g, "");
        }
      }
      // Use FileSaver to trigger the download
      const blob = response.data;
      saveAs(blob, fileName);
    } catch (error) {
      console.error("Download error:", error);
    }
  };

  const columns: ColumnsType<IStudent> = [
    {
      title: "Mã sinh viên",
      dataIndex: "studentId",
      key: "id",
    },
    {
      title: "Họ và tên",
      dataIndex: "fullName",
      key: "fullName",
    },
    {
      title: "Ngày sinh",
      dataIndex: "dateOfBirth",
      key: "dateOfBirth",
      render: (dateString: string) => {
        if (!dateString) return "";
        const date = new Date(dateString);
        return date.toLocaleDateString("vi-VN");
      },
    },
    {
      title: "Giới tính",
      dataIndex: "gender",
      key: "gender",
    },
    {
      title: "Khoa",
      dataIndex: ["faculty", "name"],
      key: "faculty",
    },
    {
      title: "Chương trình",
      dataIndex: ["program", "name"],
      key: "program",
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
    },
    {
      title: "Số điện thoại",
      dataIndex: "phoneNumber",
      key: "phone",
    },
    {
      title: "Trạng thái",
      dataIndex: ["status", "name"],
      key: "status",
    },
    {
      title: "Hình thức nhận thông báo",
      dataIndex: "subscribeToNotifications",
      key: "notification",
      render: (value: IStudentNotification[]) => {
        // Render your notification options here
        return value && value.join(", ");
      },
    },
    {
      title: "Thao tác",
      key: "action",
      render: (_, record) => (
        <Space size="middle">
          <EditOutlined
            style={{ color: "#1890ff", cursor: "pointer" }}
            onClick={() => handleUpdateStudent(record)}
          />
          <DeleteOutlined
            style={{ color: "#ff4d4f", cursor: "pointer" }}
            onClick={() => handleDeleteStudent(record.studentId)}
          />
        </Space>
      ),
    },
    {
      title: "Tải báo cáo",
      key: "download",
      render: (_, record) => {
        const menu = (
          <Menu
            onClick={({ key }) => {
              // Open the modal with the selected format
              showDownloadModal(record, Number(key));
            }}
          >
            {entries.map((type) => (
              <Menu.Item key={type[1]}>{type[0]}</Menu.Item>
            ))}
          </Menu>
        );
        return (
          <Dropdown overlay={menu}>
            <Button icon={<DownloadOutlined />}>Tải báo cáo</Button>
          </Dropdown>
        );
      },
    },
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
          student.studentId.toString().toLowerCase().includes(searchValue)) &&
        (selectedFaculty == null || student.facultyId == selectedFaculty)
      );
    });
    setFilteredStudents(filtered);
  };

  const uploadProps: UploadProps = {
    name: "file",
    accept: ".csv,.json",
    action: "http://localhost:5215/api/Student/import",
    showUploadList: false,
    onChange(info) {
      if (info.file.status === "done") {
        message.success("Import sinh viên thành công");
        updateStudents();
      } else if (info.file.status === "error") {
        message.error("Import sinh viên thất bại");
      }
    },
  };

  const downloadTemplate = (type: "csv" | "json") => {
    let content: string;
    let filename: string;
    let mimeType: string;

    if (type === "csv") {
      content = `FullName,DateOfBirth,Gender,Batch,Address,Email,PhoneNumber
Nguyễn Văn A,2000-01-15,Male,K15,123 Đường ABC,nguyenvana@example.com,0123456789`;
      filename = "student_template.csv";
      mimeType = "text/csv;charset=utf-8;";
    } else {
      content = JSON.stringify(
        {
          students: [
            {
              fullName: "Nguyễn Văn A",
              dateOfBirth: "2000-01-15",
              gender: "Male",
              batch: "K15",
              address: "123 Đường ABC",
              email: "nguyenvana@example.com",
              phoneNumber: "0123456789",
            },
          ],
        },
        null,
        2
      );
      filename = "student_template.json";
      mimeType = "application/json";
    }

    const blob = new Blob([content], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  };

  const handleExport = async (format: "csv" | "json") => {
    try {
      const response = await fetch(
        `http://localhost:5215/api/Student/export?format=${format}`
      );
      if (!response.ok) {
        throw new Error("Export failed");
      }

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `students.${format}`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);

      message.success(`Export to ${format.toUpperCase()} thành công`);
    } catch (error) {
      message.error("Export thất bại");
      console.error("Export error:", error);
    }
  };

  return (
    <>
      <Space direction="vertical" style={{ width: "100%", marginBottom: 16 }}>
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
          options={
            faculties.length > 0
              ? faculties.map((faculty) => ({
                  label: faculty.name,
                  value: faculty.id,
                }))
              : []
          }
        />
        <StudentCreateModal
          studyPrograms={studyPrograms}
          statuses={statuses}
          faculties={faculties}
          updateStudents={updateStudents}
        />
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
          <Button onClick={() => downloadTemplate("csv")}>
            Tải template CSV
          </Button>
          <Button onClick={() => downloadTemplate("json")}>
            Tải template JSON
          </Button>
          <Button onClick={() => handleExport("csv")}>Export to CSV</Button>
          <Button onClick={() => handleExport("json")}>Export to JSON</Button>
        </Space>
        <Table
          columns={columns}
          dataSource={filteredStudents}
          rowKey="studentId"
        />
      </Space>
      <Modal
        title="Chọn mục đích xác nhận và thời gian hiệu lực"
        visible={isModalVisible}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
      >
        <div style={{ marginBottom: 16 }}>
          <strong>Chọn mục đích xác nhận:</strong>
          <Radio.Group
            onChange={(e) => setSelectedReason(e.target.value)}
            value={selectedReason}
            style={{ display: "flex", flexDirection: "column", marginTop: 8 }}
          >
            {reasons.map((reason) => (
              <Radio key={reason} value={reason} style={{ marginBottom: 4 }}>
                {reason}
              </Radio>
            ))}
          </Radio.Group>
        </div>

        {selectedReason &&
          selectedReason !== "Xác nhận lý do khác" &&
          expiryDate && (
            <div style={{ marginBottom: 16 }}>
              <strong>Giấy xác nhận có hiệu lực đến ngày:</strong>
              <div>{expiryDate.format("DD/MM/YYYY")}</div>
            </div>
          )}

        {selectedReason === "Xác nhận lý do khác" && (
          <div>
            <Input
              placeholder="Ghi rõ lý do"
              value={customReason}
              onChange={(e) => setCustomReason(e.target.value)}
              style={{ marginBottom: 16 }}
            />
            <strong>Chọn thời gian hiệu lực:</strong>
            <DatePicker
              style={{ display: "block", marginTop: 8 }}
              value={customExpiryDate || undefined}
              onChange={(date) => setCustomExpiryDate(date)}
            />
          </div>
        )}
      </Modal>
    </>
  );
};

export default StudentList;
