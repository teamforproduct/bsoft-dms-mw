using System.IO;
using Aspose.Words;
using Aspose.Words.Saving;

namespace PDFCreator.Converters
{
    public class DocToPdfConverter: IPdfConverter
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