
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
        private readonly ITemplateDbProcess _operationDb;
        private InternalTemplate _document;

        public AddTemplatePaperCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplatePaper Model
        {
            get
            {
                if (!(_param is AddTemplatePaper))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplatePaper)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);
            _document = _operationDb.ModifyTemplatePaperPrepare(_context, null, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            _document.Papers = CommonDocumentUtilities.GetNewTemplatePapers(_context, Model, _document.MaxPaperOrderNumber ?? 0);

            return _operationDb.AddTemplatePapers(_context, _document.Papers);

        }


    }
}