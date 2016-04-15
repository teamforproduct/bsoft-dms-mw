using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Logic.Reports;
using BL.CrossCutting.Helpers;
using BL.Database.FileWorker;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using BL.Model.Reports.FrontModel;
using BL.Model.Constants;

namespace BL.Logic.DocumentCore.ReportsCommands
{
    public class ReportRegistrationCardDocumentCommand : BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IFileStore _fileStore;

        protected Model.DocumentCore.ReportModel.ReportDocument _reportDocument;

        public ReportRegistrationCardDocumentCommand(IDocumentsDbProcess documentDb, IFileStore fileStore)
        {
            _documentDb = documentDb;
            _fileStore = fileStore;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.IsRegistered)
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.ReportRegistrationCardDocumentPrepare(_context, Model);

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);

            _admin.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }

            _reportDocument = _documentDb.ReportRegistrationCardDocument(_context, Model);

            if (_reportDocument == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            return true;
        }

        public override object Execute()
        {
            EnumReportTypes reportType = EnumReportTypes.RegistrationCardIncomingDocument;
            switch (_document.DocumentDirection)
            {
                case EnumDocumentDirections.Incoming:
                    reportType = EnumReportTypes.RegistrationCardIncomingDocument;
                    break;
                case EnumDocumentDirections.Internal:
                    reportType = EnumReportTypes.RegistrationCardInternalDocument;
                    break;
                case EnumDocumentDirections.Outcoming:
                    reportType = EnumReportTypes.RegistrationCardOutcomingDocument;
                    break;
            }

            var filePathCrystalReport = _fileStore.GetFullTemplateReportFilePath(_context, reportType);

            var ds = new DataSetCrystalReports();
            ConvertToDataSet.ClassDataToDataTable(_reportDocument, ds, true);

            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(filePathCrystalReport);
            crystalReport.SetDataSource(ds);

            //TODO Убрать в релизе
            //Сохраняем файл для проверок
            crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, Path.Combine(new string[] { SettingConstants.FILE_STORE_DEFAULT_PATH, "report.pdf" }));

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