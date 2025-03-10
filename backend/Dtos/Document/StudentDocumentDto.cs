using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Document
{
    public class StudentDocumentDto
    {
        public string UniversityName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public string Dob { get; set; }
        public string Gender { get; set; }
        public string Faculty { get; set; }
        public string Program { get; set; }
        public string Course { get; set; }
        public string Reason { get; set; }
        public string ValidUntil { get; set; }
        public string IssueDate { get; set; }
        public string Status { get; set; }
    }
}