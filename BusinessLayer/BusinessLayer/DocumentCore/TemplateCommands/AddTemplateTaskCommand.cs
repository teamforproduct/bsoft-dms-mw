
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

        private AddTemplateDocumentTask Model
        {
            get
            {
                if (!(_param is AddTemplateDocumentTask))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateDocumentTask)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);

            if (!_operationDb.CanAddTemplateTask(_context, Model))
            {
                throw new CouldNotAddTemplateTask();
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