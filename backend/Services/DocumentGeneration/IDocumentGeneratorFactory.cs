using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.DocumentGeneration
{
    public interface IDocumentGeneratorFactory
    {
        public IDocumentGenerator CreateDocumentGenerator(DocumentType format);
    }
}