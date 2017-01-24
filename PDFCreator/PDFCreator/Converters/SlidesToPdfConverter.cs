using System.IO;
using Aspose.Slides;
using Aspose.Slides.Export;


namespace PDFCreator.Converters
{
    public class SlidesToPdfConverter : IPdfConverter
    {
        public MemoryStream ConvertToPdf(string sourceFile)
        {
            var doc = new Presentation(sourceFile);
            var res = new MemoryStream();
            doc.Save(res, SaveFormat.Pdf);
            return res;
        }
    }
}