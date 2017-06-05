
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
        private readonly ITemplateDbProcess _operationDb;


        public AddTemplateTaskCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private AddTemplateTask Model
        {
            get
            {
                if (!(_param is AddTemplateTask))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddTemplateTask)_param;
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
            var tmp = new InternalTemplateTask(Model);
            CommonDocumentUtilities.SetLastChange(_context, tmp);
            return _operationDb.AddOrUpdateTemplateTask(_context, tmp);

        }


    }
}