using System.Collections.Generic;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentTaskCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentTask _task;

        public AddDocumentTaskCommand(IDocumentOperationsDbProcess operationDb)
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

            _task = new InternalDocumentTask
            {
                Name = Model.Name,
                Description = Model.Description,
                DocumentId = Model.DocumentId,
                AgentId = Model.AgentId,
                PositionId = Model.PositionId,
                PositionExecutorAgentId = Model.PositionExecutorAgentId
            };

            return true;
        }

        public override object Execute()
        {

            CommonDocumentUtilities.SetLastChange(_context, _task);
            _operationDb.AddDocumentTasks(_context, new List<InternalDocumentTask> { _task });
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentTask;
    }
}