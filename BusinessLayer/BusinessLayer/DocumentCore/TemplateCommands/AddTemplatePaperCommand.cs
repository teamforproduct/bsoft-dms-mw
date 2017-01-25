
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplatePaperCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private InternalTemplateDocument _document;

        public AddTemplatePaperCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateDocumentPaper Model
        {
            get
            {
                if (!(_param is AddTemplateDocumentPaper))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateDocumentPaper)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            _document = _operationDb.ModifyTemplatePaperPrepare(_context, null, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            _document.Papers = CommonDocumentUtilities.GetNewTemplateDocumentPapers(_context, Model, _document.MaxPaperOrderNumber ?? 0);

            return _operationDb.AddTemplateDocumentPapers(_context, _document.Papers);

        }


    }
}