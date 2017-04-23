using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;

namespace BL.Logic.DocumentCore.Commands
{
    public class AcceptResultDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public AcceptResultDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ControlOff Model
        {
            get
            {
                if (!(_param is ControlOff))
                {
                    throw new WrongParameterTypeError();
                }
                return (ControlOff)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.Accesses!=null && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        (x.OnEvent.EventType == EnumEventTypes.MarkExecution && x.OnEvent.TargetPositionId == positionId ||
                        x.OnEvent.EventType != EnumEventTypes.MarkExecution && x.OnEvent.SourcePositionId == positionId) &&
                        x.OffEventId == null &&
                        CommonDocumentUtilities.PermissibleEventTypesForAction[CommandType].Contains(x.OnEvent.EventType))
                        .Select(x => new InternalActionRecord
                        {
                            EventId = x.OnEvent.Id,
                            WaitId = x.Id,
                            IsHideInMainMenu =  x.OnEvent.EventType == EnumEventTypes.MarkExecution && CommandType == EnumDocumentActions.CancelExecution ||
                                                x.OnEvent.EventType != EnumEventTypes.MarkExecution && CommandType == EnumDocumentActions.AcceptResult && x.IsHasMarkExecution
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
                _docWait = _document?.Waits.FirstOrDefault(x => x.OnEvent.EventType != EnumEventTypes.MarkExecution);
            }
            else
            {
                _operationDb.ControlOffMarkExecutionWaitPrepare(_context, _document);
            }
            if (_docWait?.OnEvent?.SourcePositionId == null
                || !CanBeDisplayed(_docWait.OnEvent.SourcePositionId.Value)
                )
            {
                throw new CouldNotPerformOperation();
            }
            _operationDb.ControlOffSendListPrepare(_context, _document);
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = Model.ResultTypeId;
            _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, _eventType, Model.EventDate, Model.Description, null, Model.EventId,_docWait.OnEvent.TaskId, _docWait.OnEvent.TargetPositionId, null, _docWait.OnEvent.SourcePositionId);
            _document.Waits.ToList().ForEach(x => x.OffEvent = _docWait.OffEvent);
            CommonDocumentUtilities.SetLastChange(_context, _document.Waits);
            CommonDocumentUtilities.SetLastChange(Context, _document.SendLists);
            _operationDb.CloseDocumentWait(_context, _document, GetIsUseInternalSign(), GetIsUseCertificateSign(), Model.ServerPath);
            if (_document.IsLaunchPlan)
                DmsResolver.Current.Get<IAutoPlanService>().ManualRunAutoPlan(_context, null, _document.Id);
            return _document.Id;
        }
        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), CommandType.ToString());
    }
}