using System.IO;
using Aspose.Cells;

namespace PDFCreator.Converters
{
    public class ExcelToPdfConverter : IPdfConverter
    {
        public MemoryStream ConvertToPdf(string sourceFile)
        {
            //var ext = Path.GetExtension(sourceFile);
            //LoadOptions loadOpt;
            //switch (ext.ToLower())
            //{
            //    case ".xls": loadOpt = new LoadOptions(LoadFormat.Excel97To2003); break;
            //    case ".xlsx": loadOpt = new LoadOptions(LoadFormat.Xlsx); break;
            //    case ".xlsb": loadOpt = new LoadOptions(LoadFormat.Xlsb); break;
            //    default: loadOpt = new LoadOptions(LoadFormat.Xlsx); break;
            //}
            var doc = new Workbook(sourceFile);
            var res = new MemoryStream();
            var opt = new PdfSaveOptions();
            doc.Save(res, opt);
            return res;
        }
    }
}