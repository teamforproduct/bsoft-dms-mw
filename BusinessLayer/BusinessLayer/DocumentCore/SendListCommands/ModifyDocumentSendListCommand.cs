using System.Collections.Generic;
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
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId, Model.Task, Model.Id);

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
            _sendList.TargetPositionExecutorAgentId = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, Model.TargetPositionId);
            _sendList.TargetAgentId = Model.TargetAgentId;
            _sendList.Description = Model.Description;
            _sendList.DueDate = Model.DueDate;
            _sendList.DueDay = Model.DueDay;
            _sendList.AccessLevel = Model.AccessLevel;
            _sendList.IsInitial = Model.IsInitial;
            _sendList.TaskId = taskId;
            _sendList.IsAvailableWithinTask = Model.IsAvailableWithinTask;
            _sendList.IsAddControl = Model.IsAddControl;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _sendList);

            var delPaperEvents = _document.PaperEvents.GroupJoin(Model.PaperEvents,
                pe => pe.PaperId,
                m => m.Id,
                (pe, m) => new { pe, m = m.FirstOrDefault() })
                .Where(x => x.m == null || x.pe.SourcePositionId != _sendList.SourcePositionId || x.pe.TargetPositionId != _sendList.TargetPositionId || x.pe.Description != x.m.Description)
                .Select(x => x.pe.PaperId)
                .ToList();

            var addPaperEvents = new List<InternalDocumentPaperEvent>();
            if (Model.PaperEvents?.Any(x => delPaperEvents.Contains(x.Id) || !_document.PaperEvents.Select(y => y.PaperId).ToList().Contains(x.Id)) ?? false)
                addPaperEvents.AddRange(Model.PaperEvents.Where(x => delPaperEvents.Contains(x.Id) || !_document.PaperEvents.Select(y => y.PaperId).ToList().Contains(x.Id))
                    .Select(model => CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, model.Id, EnumEventTypes.MoveDocumentPaper, model.Description, _sendList.TargetPositionId, _sendList.TargetAgentId, _sendList.SourcePositionId, _sendList.SourceAgentId, false, false)));
            addPaperEvents.ForEach(x => { x.SendListId = _sendList.Id; });
            _operationDb.ModifyDocumentSendList(_context, _sendList, _document.Tasks, addPaperEvents, delPaperEvents);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentSendList;
    }
}