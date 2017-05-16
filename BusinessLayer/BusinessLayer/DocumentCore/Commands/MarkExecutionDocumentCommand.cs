using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.Commands
{
    public class MarkExecutionDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public MarkExecutionDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
            //var markExecWaitId = _document.Waits.Where(x => x.OnEvent.EventType == EnumEventTypes.MarkExecution && !x.OffEventId.HasValue).Select(x => x.ParentId).ToList();
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.TargetPositionId == positionId &&
                        x.OnEvent.TargetPositionId != x.OnEvent.SourcePositionId &&
                        x.OffEventId == null &&
                        !x.IsHasMarkExecution &&
                        //!markExecWaitId.Contains(x.Id) &&
                        CommonDocumentUtilities.PermissibleEventTypesForAction[CommandType].Contains(x.OnEvent.EventType))
                        .Select(x => new InternalActionRecord
                        {
                            EventId = x.OnEvent.Id,
                            WaitId = x.Id
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
            _operationDb.ControlOffMarkExecutionWaitPrepare(_context, _document);
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
            var evAcceesses = (Model.TargetCopyAccessGroups?.Where(x => x.AccessType == EnumEventAccessTypes.TargetCopy) ?? new List<AccessGroup>())
                .Concat(new List<AccessGroup> { new AccessGroup { AccessType = EnumEventAccessTypes.Target, AccessGroupType = EnumEventAccessGroupTypes.Position, RecordId = _docWait.OnEvent.SourcePositionId } })
                .ToList();
            var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, EnumEventTypes.MarkExecution, Model.EventDate, Model.Description, null, Model.EventId, _docWait.OnEvent.TaskId, evAcceesses);
            CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent.Accesses);
            var newWait = CommonDocumentUtilities.GetNewDocumentWait(_context, (int)EnumEntytiTypes.Document, _document.Id, newEvent);
            newWait.ParentId = _docWait.Id;
            _document.Waits = new List<InternalDocumentWait> { newWait };

            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.AddDocumentWaits(_context, _document);
                Model.AddDocumentFiles.ForEach(x => { x.DocumentId = _document.Id; x.EventId = _document.Waits.Select(y => y.OnEventId).First(); });
                _documentProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, _context, Model.AddDocumentFiles);
                transaction.Complete();
            }
            return _document.Id;
        }


    }
}