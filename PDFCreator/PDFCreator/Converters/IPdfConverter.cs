using System.IO;

namespace PDFCreator.Converters
{
    public interface IPdfConverter
    {
        MemoryStream ConvertToPdf(string sourceFile);
    }
}