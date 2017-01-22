using System.IO;
using Aspose.Pdf;

namespace PDFCreator.Converters
{
    public class OtherToPdfConverter: IPdfConverter
    {
        public MemoryStream ConvertToPdf(string sourceFile)
        {
            var doc = new Document(sourceFile);
            var res = new MemoryStream();
            var opt = new PdfSaveOptions();
            doc.Save(res, opt);
            return res;
        }
    }
}