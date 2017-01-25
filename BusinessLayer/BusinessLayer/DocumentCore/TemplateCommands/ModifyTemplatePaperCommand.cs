
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using System.Linq;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplatePaperCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private new InternalTemplateDocument _document;
        private InternalTemplateDocumentPaper _paper;

        public ModifyTemplatePaperCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateDocumentPaper Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocumentPaper))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocumentPaper)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            _document = _operationDb.ModifyTemplatePaperPrepare(_context, Model.Id, Model);
            _paper = _document?.Papers.First();
            if (_paper == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            _paper.Name = Model.Name;
            _paper.Description = Model.Description;
            _paper.IsMain = Model.IsMain;
            _paper.IsOriginal = Model.IsOriginal;
            _paper.IsCopy = Model.IsCopy;
            _paper.PageQuantity = Model.PageQuantity;
            CommonDocumentUtilities.SetLastChange(_context, _paper);
            _operationDb.ModifyTemplatePaper(_context, _paper);
            return null;
        }


    }
}