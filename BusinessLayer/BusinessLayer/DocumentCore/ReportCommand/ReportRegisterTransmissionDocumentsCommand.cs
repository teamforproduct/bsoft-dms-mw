using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Logic.Reports;
using BL.Database.FileWorker;
using System.Collections.Generic;
using BL.Model.DocumentCore.ReportModel;

namespace BL.Logic.DocumentCore.ReportsCommands
{
    public class ReportRegisterTransmissionDocumentsCommand : BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IFileStore _fileStore;

        protected List<ReportDocument> _reportDocuments;

        public ReportRegisterTransmissionDocumentsCommand(IDocumentsDbProcess documentDb, IFileStore fileStore)
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
            //_document = _documentDb.ReportTransmissionDocumentPaperEventPrepare(_context, Model);

            //if (_document == null)
            //{
            //    throw new DocumentNotFoundOrUserHasNoAccess();
            //}
            //_context.SetCurrentPosition(_document.ExecutorPositionId);

            //_admin.VerifyAccess(_context, CommandType);

            //if (!CanBeDisplayed(_context.CurrentPositionId))
            //{
            //    throw new CouldNotPerformOperation();
            //}

            _reportDocuments = _documentDb.ReportRegisterTransmissionDocuments(_context, Model);

            if (_reportDocuments == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            return true;
        }

        public override object Execute()
        {
            return DmsReport.ReportExportToStream(_reportDocuments, _fileStore.GetFullTemplateReportFilePath(_context, EnumReportTypes.RegisterTransmissionDocuments));
        }

    }
}