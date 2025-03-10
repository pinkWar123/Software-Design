using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Document;

namespace backend.Services.DocumentGeneration
{
    public interface IDocumentGenerator
    {
        Task<byte[]> GenerateDocument(StudentDocumentDto data);
    }
}