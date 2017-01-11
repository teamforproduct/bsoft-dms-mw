using System.IO;
using Aspose.Email;
using Aspose.Email.Mail;
using Aspose.Email.Outlook;

namespace PDFCreator.Converters
{
    public class EmailToPdfConverter : IPdfConverter
    {
        public MemoryStream ConvertToPdf(string sourceFile)
        {
            var res = new MemoryStream();
            var ext = Path.GetExtension(sourceFile);
            switch (ext)
            {
                case ".msg":
                    var doc = MapiMessage.FromFile(sourceFile);
                    break;
            }
           
            return res;
        }
    }
}