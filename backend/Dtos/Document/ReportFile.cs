using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Document
{
    public class ReportFile
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; } 
        public string FileName { get; set; }
    }
}