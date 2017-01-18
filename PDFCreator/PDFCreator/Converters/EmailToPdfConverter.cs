using System;
using System.IO;
using Aspose.Email.Mail;
using Aspose.Email.Outlook;
using Aspose.Pdf;
using Aspose.Pdf.Text;

namespace PDFCreator.Converters
{
    public class EmailToPdfConverter : IPdfConverter
    {
        public MemoryStream ConvertToPdf(string sourceFile)
        {
            var res = new MemoryStream();
            var ext = Path.GetExtension(sourceFile);
            var pdfDoc = new Document();
            // Get particular page
            var pdfPage = (Page)pdfDoc.Pages[1];
            switch (ext)
            {
                case ".msg":
                case ".eml":
                case ".emlx":
                    var eml = MailMessage.Load(sourceFile);
                    switch (eml.BodyType)
                    {
                        case BodyContentType.PlainText:
                            var textFragment = new TextFragment(eml.Body);
                            textFragment.Position = new Position(60, 60);
                            textFragment.TextState.FontSize = 12;
                            textFragment.TextState.Font = FontRepository.FindFont("TimesNewRoman");
                            textFragment.TextState.BackgroundColor = Color.White;
                            textFragment.TextState.ForegroundColor = Color.Black;
                            TextBuilder textBuilder = new TextBuilder(pdfPage);
                            textBuilder.AppendText(textFragment);
                            break;
                        case BodyContentType.Rtf:
                        case BodyContentType.Html:
                            var tmpFile = Path.GetTempFileName();
                            File.WriteAllText(tmpFile, eml.HtmlBody);
                            pdfDoc = new Document(tmpFile, new Aspose.Pdf.HtmlLoadOptions());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }
            pdfDoc.Save(res, new PdfSaveOptions());

            return res;
        }
    }
}