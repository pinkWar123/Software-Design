using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IStudentService
    {
        Task ImportFromCsv(Stream stream);
        Task ImportFromJson(Stream stream);
        Task<byte[]> ExportToCsv();
        Task<byte[]> ExportToJson();
    }
}