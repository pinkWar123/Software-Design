using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Document;
using DinkToPdf;

namespace backend.Services.DocumentGeneration
{
    public class PDFDocumentGenerator : IDocumentGenerator
    {
        private readonly HtmlDocumentGenerator _htmlGenerator;
        private readonly SynchronizedConverter _converter;

        public PDFDocumentGenerator(
            HtmlDocumentGenerator htmlGenerator,
            SynchronizedConverter converter)
        {
            _htmlGenerator = htmlGenerator;
            _converter = converter;
        }

        public async Task<byte[]> GenerateDocument(StudentDocumentDto data)
        {
            // Generate the HTML first
            byte[] htmlBytes = await _htmlGenerator.GenerateDocument(data);
            string htmlContent = System.Text.Encoding.UTF8.GetString(htmlBytes);

            // Convert HTML to PDF using a PDF conversion library (e.g., DinkToPdf, wkhtmltopdf, etc.)
            // This is pseudocode: replace with your library's method.
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4Plus,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            
            byte[] pdfBytes = _converter.Convert(doc);
            return pdfBytes;
        }
    }
}