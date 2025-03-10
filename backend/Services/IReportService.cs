using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Document;
using backend.Services.DocumentGeneration;

namespace backend.Services
{
    public interface IReportService
    {
        Task<ReportFile?> GenerateReportForStudent(int studentId, DocumentType format, string reason, string expiredDate);
    }
}