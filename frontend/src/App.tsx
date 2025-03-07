import './App.css'
import StudentList from './components/StudentList'
import FacultyList from './components/Faculty'
import { useEffect, useState } from 'react';
import IStudent from './models/Student';
import IProgram from './models/Program';
import IStatus from './models/Status';
import { callGetAllStudents } from './services/student';
import { callGetStudyPrograms } from './services/studyProgram';
import { callGetAllStatuses } from './services/status';
import IFaculty from './models/Faculty';
import Program from './components/Program';
import Status from './components/Status';
import {App as AntdApp, Avatar, message, Typography} from 'antd';
import VersionInfo from './components/VersionInfo';
import { IConfiguration } from './models/Configuration';
import { callGetAllConfigurations } from './services/configuration';
import Configuration from './components/Configuration';
import { Header } from 'antd/es/layout/layout';
function App() {
    const [messageApi, contextHolder] = message.useMessage();
    
  const [students, setStudents] = useState<IStudent[]>([]);
  const [studyPrograms, setStudyPrograms] = useState<IProgram[]>([]);
  const [statuses, setStatuses] = useState<IStatus[]>([]);
  const [faculties, setFaculties] = useState<IFaculty[]>([]);
  const [configurations, setConfigurations] = useState<IConfiguration[]>([]);
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
    const fetchConfigurations  = async () => {
        try {
            const response = await callGetAllConfigurations();
            setConfigurations(response);
        } catch (error) {
            console.error('Error fetching configurations:', error);
        }

        fetchConfigurations();
    }
  }, []);

  const updateStudents = async () => {
    const students = await callGetAllStudents();
    setStudents(students);
  }


  
  return (
    <AntdApp>
    <>
        <Header style={{
            display:'flex',
            justifyContent: 'space-between',
        color: '#fff',
        height: 100,
        paddingInline: 48,
        lineHeight: '64px',
        backgroundColor: '#4096ff',
        marginBottom: '20px',
        borderRadius: '20px'
        }}>
            <Typography.Title style={{color: 'white'}} level={2}>Trường đại học khoa học tự nhiên</Typography.Title>
            <Avatar style={{width: '100px', height: '100px', color:"white"}} src="https://hcmus.edu.vn/wp-content/uploads/2021/12/logo-khtn_remake-1.png"/>
        </Header>
      {contextHolder}
      <StudentList students={students} studyPrograms={studyPrograms} statuses={statuses} faculties={faculties} updateStudents={updateStudents} />
      <FacultyList faculties={faculties} updateFaculties={setFaculties}/>
      <Program programs={studyPrograms} updatePrograms={setStudyPrograms}/>
      <Status statuses={statuses} updateStatuses={setStatuses}/>
      <div style={{paddingBottom: '200px'}}>
        <Configuration configurations={configurations} updateConfigurations={setConfigurations}/>

    </div>
        <VersionInfo />
      
    </>
    </AntdApp>
  )
}

export default App
