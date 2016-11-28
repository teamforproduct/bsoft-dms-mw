
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class AddTemplateTaskCommand : BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;


        public AddTemplateTaskCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateDocumentTask Model
        {
            get
            {
                if (!(_param is ModifyTemplateDocumentTask))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateDocumentTask)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            if (!_operationDb.CanAddTemplateTask(_context, Model))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            return true;
        }

        public override object Execute()
        {
            var tmp = new InternalTemplateDocumentTask(Model);
            CommonDocumentUtilities.SetLastChange(_context, tmp);
            return _operationDb.AddOrUpdateTemplateTask(_context, tmp);

        }


    }
}