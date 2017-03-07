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

        private ModifyDocumentTask Model
        {
            get
            {
                if (!(_param is ModifyDocumentTask))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentTask)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Tasks.Where(
                    x => x.PositionId == positionId
                        )
                        .Select(x => new InternalActionRecord
                        {
                            TaskId = x.Id,
                        });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ModifyDocumentTaskPrepare(_context, Model.Id, Model);
            _task = _document?.Tasks.First();
            if (_task == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.Tasks.Count()>1)
            {
                throw new RecordNotUnique();
            }
            _context.SetCurrentPosition(_task.PositionId);
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperationWithPaper();
            }
            return true;
        }

        public override object Execute()
        {
            if (_task.Name != Model.Name || _task.Description != Model.Description)
            {
                Document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, null, EnumEventTypes.TaskFormulation, null, Model.Name + " / " + Model.Description);
            }
            _task.Name = Model.Name;
            _task.Description = Model.Description;
            CommonDocumentUtilities.SetLastChange(_context, _task);
            _operationDb.ModifyDocumentTask(_context, _document);
            return null;
        }

    }
}