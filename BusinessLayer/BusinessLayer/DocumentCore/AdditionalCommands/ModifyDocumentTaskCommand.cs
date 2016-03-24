using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class ModifyDocumentTaskCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentTask _task;

        public ModifyDocumentTaskCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ModifyDocumentTasks Model
        {
            get
            {
                if (!(_param is ModifyDocumentTasks))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentTasks)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);

            _task = _operationDb.ChangeDocumentTaskPrepare(_context, Model.Id);

           //TODO Проверить поля которые нужно обновлять
            _task.DocumentId = Model.DocumentId;
            _task.PositionId = Model.PositionId;
            _task.PositionExecutorAgentId = Model.PositionExecutorAgentId;
            _task.AgentId = Model.AgentId;
            _task.Name = Model.Name;
            _task.Description = Model.Description;

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _task);
            _operationDb.ModifyDocumentTask(_context, _task);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentTask;
    }
}