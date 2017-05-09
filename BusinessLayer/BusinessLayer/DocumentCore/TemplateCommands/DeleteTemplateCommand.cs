using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class DeleteTemplateCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private readonly IFileStore _fStore;
        private new InternalTemplateDocument _document;
        public DeleteTemplateCommand(ITemplateDocumentsDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
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
            _adminProc.VerifyAccess(_context, CommandType, false);
            _document = _operationDb.DeleteTemplatePrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            if (!_operationDb.CanModifyTemplate(_context, Model))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            return true;
        }

        public override object Execute()
        {
            if ((_document?.FileCount ?? 0) > 0)
            {
                _fStore.DeleteAllFileInTemplate(_context, _document.Id);
            }
            _operationDb.DeleteTemplate(_context, Model);
            return null;
        }

    }
}