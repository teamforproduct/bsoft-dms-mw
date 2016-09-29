using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.Model.Constants;
using BL.Model.Reports.FrontModel;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BL.Database.Reports
{
    public class DmsReport
    {
        public FrontReport ReportExportToStream<T>(T data, string filePathCrystalReport) where T : class
        {
            return ReportExportToStream(new List<T> { data }, filePathCrystalReport);
        }
        public FrontReport ReportExportToStream<T>(List<T> data, string filePathCrystalReport) where T : class
        {
            var ds = new DataSetCrystalReports();

            var convertService = DmsResolver.Current.Get<IConvertToDataSet>();

            convertService.ClassListToDataTable(data, ds, true);

            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(filePathCrystalReport);
            crystalReport.SetDataSource(ds);

            //TODO Убрать в релизе
            //Сохраняем файл для проверок
            //crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, Path.Combine(new string[] { SettingConstants.FILE_STORE_DEFAULT_PATH, "report.pdf" }));

            var stream = crystalReport.ExportToStream(ExportFormatType.PortableDocFormat);

            var res = new FrontReport();

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                res.FileContent = ms.ToArray();
            }

            return res;
        }
    }
}
