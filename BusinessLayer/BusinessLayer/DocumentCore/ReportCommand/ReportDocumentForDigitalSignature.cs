using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Database.FileWorker;
using BL.Model.DocumentCore.InternalModel;
using BL.CrossCutting.DependencyInjection;
using BL.Database.Reports;
using BL.Model.DocumentCore.IncomingModel;
using BL.Logic.DocumentCore.Interfaces;

namespace BL.Logic.DocumentCore.ReportsCommands
{
    public class ReportDocumentForDigitalSignature : BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IFileStore _fileStore;

        public ReportDocumentForDigitalSignature(IDocumentsDbProcess documentDb, IFileStore fileStore)
        {
            _documentDb = documentDb;
            _fileStore = fileStore;
        }

        private DigitalSignatureDocumentPdf Model
        {
            get
            {
                if (!(_param is DigitalSignatureDocumentPdf))
                {
                    throw new WrongParameterTypeError();
                }
                return (DigitalSignatureDocumentPdf)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.ReportDocumentForDigitalSignaturePrepare(_context, Model);

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            _admin.VerifyAccess(_context, CommandType);

            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }

            return true;
        }

        public override object Execute()
        {
            var file = _documentDb.ReportDocumentForDigitalSignature(_context, Model, GetIsUseInternalSign(), GetIsUseCertificateSign());

            return file;
        }
    }
}