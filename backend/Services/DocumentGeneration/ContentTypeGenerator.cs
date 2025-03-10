using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.DocumentGeneration
{
    public static class ContentTypeGenerator
    {
        public static string GetContentType(DocumentType format)
        {
            string contentType = format switch
            {
                DocumentType.HTML => "text/html",
                DocumentType.Markdown => "text/markdown",
                DocumentType.PDF => "application/pdf",
                // DocumentType.Docx => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
            
            return contentType;
        }

        public static string GetExtension(DocumentType format)
        {
            string extension = format switch
            {
                DocumentType.HTML => "html",
                DocumentType.Markdown => "md",
                DocumentType.PDF => "pdf",
                // DocumentType.Docx => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "octet-stream"
            };
            
            return extension;
        }
    }
}