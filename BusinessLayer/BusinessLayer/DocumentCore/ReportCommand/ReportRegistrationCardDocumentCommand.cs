using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Logic.Reports;
using BL.Database.FileWorker;
using BL.Model.DocumentCore.ReportModel;

namespace BL.Logic.DocumentCore.ReportsCommands
{
    public class ReportRegistrationCardDocumentCommand : BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IFileStore _fileStore;

        protected ReportDocument _reportDocument;

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

            return DmsReport.ReportExportToStream(_reportDocument, _fileStore.GetFullTemplateReportFilePath(_context, reportType));
        }
    }
}