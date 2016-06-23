using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.DocumentCore.Interfaces;
using System.Transactions;

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
            if (!_actionRecords.Any() /*|| _document.IsLaunchPlan*/)
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ChangeDocumentSendListPrepare(_context, Model.DocumentId, Model.Task, Model.Id);

            _sendList = _document?.SendLists.FirstOrDefault(x => x.Id == Model.Id);
            if (_sendList == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (Model.IsInitial || _sendList.IsInitial)
            {
                _context.SetCurrentPosition(_document.ExecutorPositionId);
            }
            else
            {
                _context.SetCurrentPosition(_sendList.SourcePositionId);
            }
            _admin.VerifyAccess(_context, CommandType);
            var tmpSendList = _document.SendLists;
            _document.SendLists = _document?.SendLists.Where(x => x.Id == Model.Id).ToList();
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }
            _document.SendLists = tmpSendList;

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
            _sendList.IsWorkGroup = Model.IsWorkGroup;
            _sendList.IsAddControl = Model.IsAddControl;
            _sendList.SelfDueDate = Model.SelfDueDate;
            _sendList.SelfDueDay = Model.SelfDueDay;
            _sendList.SelfAttentionDate = Model.SelfAttentionDate;

            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _sendList);
            var delPaperEvents = new List<int?>();
            if (_document.PaperEvents?.Any() ?? false)
            {
                if (Model.PaperEvents?.Any() ?? false)
                {
                    delPaperEvents = _document.PaperEvents.GroupJoin(Model.PaperEvents,
                       pe => pe.PaperId,
                       m => m.Id,
                       (pe, m) => new { pe, m = m.FirstOrDefault() })
                       .Where(
                           x =>
                               x.m == null || x.pe.SourcePositionId != _sendList.SourcePositionId ||
                               x.pe.TargetPositionId != _sendList.TargetPositionId || x.pe.Description != x.m.Description)
                       .Select(x => x.pe.PaperId)
                       .ToList();
                }
                else
                {
                    delPaperEvents = _document.PaperEvents.Select(x => x.PaperId).ToList();
                }
            }
            var addPaperEvents = new List<InternalDocumentEvent>();
            if (Model.PaperEvents?.Any(x => delPaperEvents.Contains(x.Id) || !_document.PaperEvents.Select(y => y.PaperId).ToList().Contains(x.Id)) ?? false)
                addPaperEvents.AddRange(Model.PaperEvents.Where(x => delPaperEvents.Contains(x.Id) || !_document.PaperEvents.Select(y => y.PaperId).ToList().Contains(x.Id))
                                                            .Select(model => CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, Model.DocumentId, model.Id, EnumEventTypes.MoveDocumentPaper, model.Description, _sendList.TargetPositionId, _sendList.TargetAgentId, _sendList.SourcePositionId, _sendList.SourceAgentId, false, false)));
            addPaperEvents.ForEach(x => { x.SendListId = _sendList.Id; });
//            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                _operationDb.ModifyDocumentSendList(_context, _sendList, _document.Tasks, addPaperEvents, delPaperEvents);
                if (Model.IsLaunchItem ?? false)
                {
                    var aplan = DmsResolver.Current.Get<IAutoPlanService>();
                    aplan.ManualRunAutoPlan(_context, _sendList.Id, _document.Id);
                }
                else
                {
                    var aplan = DmsResolver.Current.Get<IAutoPlanService>();
                    aplan.ManualRunAutoPlan(_context, null, _document.Id);
                }
//                transaction.Complete();
            }
            return null;
        }

    }
}