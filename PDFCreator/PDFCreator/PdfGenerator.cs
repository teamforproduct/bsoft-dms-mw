using System;
using System.IO;
using GhostscriptSharp;
using PDFCreator.Converters;

namespace PDFCreator
{
    public static class PdfGenerator
    {
        public static bool CreatePdf(string inputFile, string outputFile)
        {
            if (string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
                throw new ArgumentException("Wrong input file path!");
            if (string.IsNullOrEmpty(outputFile)) throw new ArgumentException("Output path could not be empty!");

            try
            {
                var converter = ConverterFactory.GetConverter(inputFile);
                var pdf = converter.ConvertToPdf(inputFile);
                using (var fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    pdf.WriteTo(fileStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CreatePdfPreview(string inputFile, string outputFile)
        {
            if (string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
                throw new ArgumentException("Wrong input file path!");
            if (string.IsNullOrEmpty(outputFile)) throw new ArgumentException("Output path could not be empty!");

            try
            {
                GhostscriptWrapper.GeneratePageThumb(inputFile, outputFile, 1, 300, 300);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

    }
}
