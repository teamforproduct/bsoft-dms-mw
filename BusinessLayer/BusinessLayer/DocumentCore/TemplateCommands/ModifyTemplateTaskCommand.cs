
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
        private readonly ITemplateDocumentsDbProcess _operationDb;

        public ModifyTemplateTaskCommand(ITemplateDocumentsDbProcess operationDb)
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