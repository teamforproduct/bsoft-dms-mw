using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.SendListCommands
{
    public class ModifyDocumentSendListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentSendList _sendList;

        public ModifyDocumentSendListCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ModifyDocumentSendList Model
        {
            get
            {
                if (!(_param is ModifyDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.SendLists.Where(
                    x =>
                        x.SourcePositionId == positionId
                        && x.StartEventId == null && x.CloseEventId == null)
                                                .Select(x => new InternalActionRecord
                                                {
                                                    SendListId = x.Id,
                                                });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId,Model.Task);

            _sendList = _document?.SendLists.FirstOrDefault(x => x.Id == Model.Id);
            if (_sendList == null || !CanBeDisplayed(_sendList.SourcePositionId))
            {
                throw new CouldNotPerformThisOperation();
            }
            _context.SetCurrentPosition(_sendList.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            var taskId = CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Task); //TODO исправление от кого????
            _sendList.Stage = Model.Stage;
            _sendList.SendType = Model.SendType;
            _sendList.TargetPositionId = Model.TargetPositionId;
            //TODO ???? _sendList.Task = Model.Task;
            _sendList.Description = Model.Description;
            _sendList.DueDate = Model.DueDate;
            _sendList.DueDay = Model.DueDay;
            _sendList.AccessLevel = Model.AccessLevel;
            _sendList.IsInitial = Model.IsInitial;
            _sendList.TaskId = taskId;
            _sendList.IsAvailableWithinTask = Model.IsAvailableWithinTask;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _sendList);
            _operationDb.ModifyDocumentSendList(_context, _sendList);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentSendList;
    }
}