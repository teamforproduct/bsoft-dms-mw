using System;
using System.IO;

namespace PDFCreator.Converters
{
    public static class ConverterFactory
    {
        public static IPdfConverter GetConverter(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".doc":
                case ".docx":
                case ".rtf":
                case ".html":
                case ".mhtml":
                case ".odt":
                case ".ott":
                case ".txt":
                case ".epub":return new DocToPdfConverter();

                case ".xls":
                case ".xlsx":
                case ".xlsb":
                case ".csv":
                case ".ods": return new ExcelToPdfConverter();

                case ".xps":
                case ".xml":
                case ".svg": return new OtherToPdfConverter();

                default: throw new FormatException("Unknown file format!");
            }
        }
    }
}