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
import {App as AntdApp} from 'antd';
function App() {
  const [students, setStudents] = useState<IStudent[]>([]);
  const [studyPrograms, setStudyPrograms] = useState<IProgram[]>([]);
  const [statuses, setStatuses] = useState<IStatus[]>([]);
  const [faculties, setFaculties] = useState<IFaculty[]>([]);
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
  const updateStudents = async () => {
    const students = await callGetAllStudents();
    setStudents(students);
  }


  
  return (
    <AntdApp>
      <StudentList students={students} studyPrograms={studyPrograms} statuses={statuses} faculties={faculties} updateStudents={updateStudents} />
      <FacultyList faculties={faculties} updateFaculties={setFaculties}/>
      <Program programs={studyPrograms} updatePrograms={setStudyPrograms}/>
      <Status statuses={statuses} updateStatuses={setStatuses}/>
    </AntdApp>
  )
}

export default App
