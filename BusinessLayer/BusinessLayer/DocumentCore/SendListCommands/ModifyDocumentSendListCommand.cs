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
using BL.Model.AdminCore;

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
            _adminProc.VerifyAccess(_context, CommandType);

            if (Model.TargetPositionId.HasValue
                && (_document.RestrictedSendLists?.Any() ?? false)
                && !_document.RestrictedSendLists.Select(x => x.PositionId).Contains(Model.TargetPositionId.Value)
                )
            {
                throw new DocumentSendListNotFoundInDocumentRestrictedSendList();
            }

            var tmpSendList = _document.SendLists;
            _document.SendLists = _document?.SendLists.Where(x => x.Id == Model.Id).ToList();
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformOperation();
            }
            _document.SendLists = tmpSendList;
            
            var taskId = CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Task); //TODO исправление от кого????
            CommonDocumentUtilities.SetSendListAtrributes(_context, _sendList, Model, taskId);
            CommonDocumentUtilities.VerifySendLists(_document);

            return true;
        }

        public override object Execute()
        {
            if (Model.TargetPositionId.HasValue
                && (Model.DueDate.HasValue || Model.DueDay.HasValue)
                && (_sendList.SendType == EnumSendTypes.SendForSigning || _sendList.SendType == EnumSendTypes.SendForVisaing || _sendList.SendType == EnumSendTypes.SendForАgreement || _sendList.SendType == EnumSendTypes.SendForАpproval)
                && !_adminProc.VerifySubordination(_context, new VerifySubordination
                {
                    SubordinationType = EnumSubordinationTypes.Execution,
                    TargetPosition = new List<int> { Model.TargetPositionId.Value },//TODO test
                    SourcePositions = CommonDocumentUtilities.GetSourcePositionsForSubordinationVeification(_context, _sendList, _document, true),
                }))
            {
                _sendList.AddDescription = "##l@DmsExceptions:SubordinationForDueDateHasBeenViolated@l##";
            }

            #region Paper
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
                                                            .Select(model => CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, (int)EnumEntytiTypes.Document, Model.DocumentId, model.Id, EnumEventTypes.MoveDocumentPaper, model.Description, _sendList.TargetPositionId, _sendList.TargetAgentId, _sendList.SourcePositionId, _sendList.SourceAgentId, false, false)));
            addPaperEvents.ForEach(x => { x.SendListId = _sendList.Id; });
            #endregion Paper
            //            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.ModifyDocumentSendList(_context, _sendList, _document.Tasks, addPaperEvents, delPaperEvents);
                if (Model.IsLaunchItem ?? false)
                    DmsResolver.Current.Get<IAutoPlanService>().ManualRunAutoPlan(_context, _sendList.Id, _document.Id);
                else
                    DmsResolver.Current.Get<IAutoPlanService>().ManualRunAutoPlan(_context, null, _document.Id);

                //                transaction.Complete();
            }
            return null;
        }

    }
}