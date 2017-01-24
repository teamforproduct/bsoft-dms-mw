using System;
using System.IO;

namespace PDFCreator.Converters
{
    public static class ConverterFactory
    {
        public static string[] SupportedTypes =
        {
                ".doc",
                ".docx",
                ".dot",
                ".dotx",
                ".dotm",
                ".rtf",
                ".html",
                ".mhtml",
                ".odt",
                ".ott",
                ".txt",
                ".epub",

                ".xls",
                ".xlsx",
                ".xlsb",
                ".csv",
                ".ods", 

                ".msg",
                //".pst",
                //".ost",
                //".oft",
                //".mbox",
                ".emlx",
                ".eml",

                ".ppt",
                ".pptx",
                ".pps",
                ".ppsx",
                ".pptm",
                ".ppsm",
                ".potx",
                ".potm",
                ".odp",

                ".xps",
                ".xml",
                ".svg"
    };


        public static IPdfConverter GetConverter(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".doc":
                case ".docx":
                case ".dot":
                case ".dotx":
                case ".dotm":
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

                case ".msg":
                //case ".pst":
                //case ".ost":
                //case ".oft":
                //case ".mbox":
                case ".emlx":
                case ".eml": return new EmailToPdfConverter();

                case ".ppt":
                case ".pptx":
                case ".pps":
                case ".ppsx":
                case ".pptm":
                case ".ppsm":
                case ".potx":
                case ".potm":
                case ".odp": return new SlidesToPdfConverter();

                case ".xps":
                case ".xml":
                case ".svg": return new OtherToPdfConverter();

                default: throw new FormatException("Unknown file format!");
            }
        }
    }
}