﻿using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using System.Collections.Generic;
using BL.Model.AdminCore;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DocumentCore.Commands
{
    public class ControlOffDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentWait _docWait;

        public ControlOffDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            _actionRecords =
                _document.Waits.Where(
                    x =>
                        x.OnEvent.SourcePositionId == positionId &&
                        x.OffEventId == null &&
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
            _document = _operationDb.ControlChangeDocumentPrepare(_context, Model.EventId);
            _docWait = _document?.Waits?.FirstOrDefault();
            if (_docWait?.OnEvent?.SourcePositionId == null || !CanBeDisplayed(_docWait.OnEvent.SourcePositionId.Value))
            {
                throw new CouldNotPerformOperation();
            }
            _operationDb.SetRestrictedSendListsPrepare(_context, _document);
            _operationDb.SetParentEventAccessesPrepare(_context, _document, Model.EventId);
            _context.SetCurrentPosition(_docWait.OnEvent.SourcePositionId);
            _adminProc.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _docWait.ResultTypeId = Model.ResultTypeId;
            var evAcceesses = (Model.TargetCopyAccessGroups?.Where(x => x.AccessType == EnumEventAccessTypes.TargetCopy) ?? new List<AccessGroup>())
                .Concat(new List<AccessGroup> { new AccessGroup { AccessType = EnumEventAccessTypes.Target, AccessGroupType = EnumEventAccessGroupTypes.Position, RecordId = _docWait.OnEvent.TargetPositionId } })
                .ToList();
            var newEvent = _docWait.OffEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, (int)EnumEntytiTypes.Document, _docWait.DocumentId, EnumEventTypes.ControlOff, Model.EventDate, Model.Description, null, Model.EventId, _docWait.OnEvent.TaskId, evAcceesses);
            CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent.Accesses);
            CommonDocumentUtilities.SetLastChange(_context, _docWait);
            using (var transaction = Transactions.GetTransaction())
            {
                _operationDb.CloseDocumentWait(_context, _document, GetIsUseInternalSign(), GetIsUseCertificateSign(), Model.ServerPath);
                Model.AddDocumentFiles.ForEach(x => { x.DocumentId = _document.Id; x.EventId = _document.Waits.Select(y => y.OffEventId).First(); });
                _documentProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, _context, Model.AddDocumentFiles);
                transaction.Complete();
            }
            return _docWait.DocumentId;
        }

    }
}