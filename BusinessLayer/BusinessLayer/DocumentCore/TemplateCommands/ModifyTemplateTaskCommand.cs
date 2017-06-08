
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class ModifyTemplateTaskCommand : BaseDocumentCommand
    {
        private readonly ITemplateDbProcess _operationDb;

        public ModifyTemplateTaskCommand(ITemplateDbProcess operationDb)
        {
            _operationDb = operationDb;

        }

        private ModifyTemplateTask Model
        {
            get
            {
                if (!(_param is ModifyTemplateTask))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyTemplateTask)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminProc.VerifyAccess(_context, CommandType, false);


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