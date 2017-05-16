using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.AdminCore;
using System.Collections.Generic;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlTargetChangeDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public ControlTargetChangeDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ControlTargetChange Model
        {
            get
            {
                if (!(_param is ControlTargetChange))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlTargetChange)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.TargetPositionId == positionId &&
                        x.OffEventId == null &&
                        CommonDocumentUtilities.PermissibleEventTypesForAction[CommandType].Contains(x.OnEvent.EventType))
                        .Select( x=>new InternalActionRecord
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
            _document = _operationDb.ControlTargetChangeDocumentPrepare(_context, Model.EventId);
            _docWait = _document?.Waits?.FirstOrDefault();
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
            var addDescripton = (Model.TargetDescription != _docWait.TargetDescription ? "формулировка задачи" + "," : "")
                    + ((Model.TargetAttentionDate != _docWait.AttentionDate ? "дата постоянного внимания" + "," : ""));
            if (!string.IsNullOrEmpty(addDescripton))
            {
                addDescripton = addDescripton.Remove(addDescripton.Length - 1);
                _docWait.TargetDescription = Model.TargetDescription;
                _docWait.AttentionDate = Model.TargetAttentionDate;
                CommonDocumentUtilities.SetLastChange(_context, _docWait);
                var newEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, EnumEventTypes.ControlTargetChange, Model.EventDate, Model.TargetDescription, addDescripton, Model.EventId, _docWait.OnEvent.TaskId, Model.TargetCopyAccessGroups);
                CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent.Accesses);
                _document.Events = new List<InternalDocumentEvent> { newEvent };
                using (var transaction = Transactions.GetTransaction())
                {
                    _operationDb.ChangeTargetDocumentWait(_context, _document);
                    Model.AddDocumentFiles.ForEach(x => { x.DocumentId = _document.Id; x.EventId = _document.Events.Select(y => y.Id).First(); });
                    _documentProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, _context, Model.AddDocumentFiles);
                    transaction.Complete();
                }
            }
            else
                throw new ContriolHasNotBeenChanged();
            return _docWait.DocumentId;
        }

    }
}