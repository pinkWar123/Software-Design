using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;

namespace backend.Services.DocumentGeneration
{
    public class DocumentGeneratorFactory : IDocumentGeneratorFactory
    {
        private readonly SynchronizedConverter _converter;
        public DocumentGeneratorFactory(SynchronizedConverter converter)
        {
            _converter = converter;
        }
        public IDocumentGenerator CreateDocumentGenerator(DocumentType format)
        {
            var htmlGenerator = new HtmlDocumentGenerator();

            switch (format)
            {
                case DocumentType.HTML:
                    return htmlGenerator;
                case DocumentType.Markdown:
                    return new MarkdownDocumentGenerator();
                case DocumentType.PDF:
                    return new PDFDocumentGenerator(htmlGenerator, _converter);
                default:
                    throw new NotSupportedException("Document format not supported.");
            }
        }
    }
}