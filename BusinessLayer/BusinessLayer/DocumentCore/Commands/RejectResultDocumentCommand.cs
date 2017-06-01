using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Collections.Generic;
using BL.Model.DocumentCore.IncomingModel;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.Commands
{
    public class RejectResultDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public RejectResultDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private SendEventMessage Model
        {
            get
            {
                if (!(_param is SendEventMessage))
                {
                    throw new WrongParameterTypeError();
                }
                return (SendEventMessage)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        (x.OnEvent.EventType == EnumEventTypes.MarkExecution && x.OnEvent.TargetPositionId == positionId ||
                        x.OnEvent.EventType != EnumEventTypes.MarkExecution && x.OnEvent.EventType != EnumEventTypes.AskPostponeDueDate && x.IsHasMarkExecution && x.OnEvent.SourcePositionId == positionId) &&
                        x.OffEventId == null &&
                        CommonDocumentUtilities.PermissibleEventTypesForAction[CommandType].Contains(x.OnEvent.EventType))
                        .Select(x => new InternalActionRecord
                        {
                            EventId = x.OnEvent.Id,
                            WaitId = x.Id,
                            IsHideInMainMenu = x.OnEvent.EventType != EnumEventTypes.MarkExecution
                        });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ControlOffDocumentPrepare(_context, Model.EventId);
            _docWait = _document?.Waits.FirstOrDefault();

            if (_docWait == null)
            {
                throw new CouldNotPerformOperation();
            }
            if (_docWait.OnEvent.EventType == EnumEventTypes.MarkExecution && _docWait.ParentOnEventId.HasValue)
            {
                ((List<InternalDocumentWait>)_document.Waits).AddRange(_operationDb.ControlOffDocumentPrepare(_context, _docWait.ParentOnEventId.Value).Waits);
            }
            else
            {
                _operationDb.ControlOffMarkExecutionWaitPrepare(_context, _document);
                _docWait = _document?.Waits.FirstOrDefault(x => x.OnEvent.EventType == EnumEventTypes.MarkExecution);
            }

            if (_docWait?.OnEvent?.TargetPositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.TargetPositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            _operationDb.SetRestrictedSendListsPrepare(_context, _document);
            _operationDb.SetParentEventAccessesPrepare(_context, _document, Model.EventId);
            _context.SetCurrentPosition(_docWait.OnEvent.TargetPositionId);
            _adminProc.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = (int)EnumResultTypes.CloseByRejecting;
            var evAcceesses = (Model.TargetCopyAccessGroups?.Where(x => x.AccessType == EnumEventAccessTypes.TargetCopy) ?? new List<AccessGroup>())
                .Concat(new List<AccessGroup> { new AccessGroup { AccessType = EnumEventAccessTypes.Target, AccessGroupType = EnumEventAccessGroupTypes.Position, RecordId = _docWait.OnEvent.SourcePositionId } })
                .Concat(CommonDocumentUtilities.GetAccessGroupsFileExecutors(_context, _document.Id, Model.AddDocumentFiles))
                .ToList();
            var newEvent = _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, EnumEventTypes.RejectResult, Model.EventDate, 
                                                                                            Model.Description, null, _docWait.OnEvent.Id, _docWait.OnEvent.TaskId, evAcceesses);
            CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent.Accesses);
            CommonDocumentUtilities.SetLastChange(_context, _docWait);
            _document.Waits = new List<InternalDocumentWait> { _docWait };
            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.CloseDocumentWait(_context, _document, GetIsUseInternalSign(), GetIsUseCertificateSign(), Model.ServerPath);
                if (Model.AddDocumentFiles?.Any() ?? false)
                {
                    Model.AddDocumentFiles.ForEach(x => { x.DocumentId = _document.Id; x.EventId = _document.Waits.Select(y => y.OffEventId).First(); });
                    _documentProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, _context, Model.AddDocumentFiles);
                }
                transaction.Complete();
            }
            return _document.Id;
        }

    }
}