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
    public class DeleteTemplateFileCommand: BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;
        private readonly IFileStore _fStore;

        public DeleteTemplateFileCommand(ITemplateDocumentsDbProcess operationDb, IFileStore fStore)
        {
            _operationDb = operationDb;
            _fStore = fStore;
        }

        private ModifyTemplateAttachedFile Model
        {
            get
            {
                if (!(_param is ModifyTemplateAttachedFile))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateAttachedFile)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            if (Model.DocumentId <= 0 || Model.OrderInDocument <= 0)
            {
                throw new WrongParameterValueError();
            }
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var docFile = new InternalTemplateAttachedFile
            {
                DocumentId = Model.DocumentId,
                OrderInDocument = Model.OrderInDocument
            };

            _fStore.DeleteFile(_context, docFile);
            _operationDb.DeleteTemplateAttachedFile(_context, docFile);
            return null;
        }
       
    }
}