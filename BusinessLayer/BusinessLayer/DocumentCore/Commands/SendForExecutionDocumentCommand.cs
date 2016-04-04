using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForExecutionDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForExecutionDocumentCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private InternalDocumentSendList Model
        {
            get
            {
                if (!(_param is InternalDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                var model = (InternalDocumentSendList)_param;
                if (model.SendType != EnumSendTypes.SendForExecution)
                {
                    throw new WrongParameterTypeError();
                }
                return (InternalDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return false;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(Model.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);   //TODO без позиций
            _document = _operationDb.SendForExecutionDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (Model.StartEventId != null || Model.CloseEventId != null)
            {
                throw new PlanPointHasAlredyBeenLaunched();
            }
            if (!Model.TargetPositionId.HasValue || _document.Waits == null|| _document.Waits.Count()>1)
            {
                throw new WrongDocumentSendListEntry();
            }
            CommonDocumentUtilities.PlanDocumentPaperFromSendList(_context, _document, Model);
            return true;
        }

        public override object Execute()
        {
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);

            var waitParent = _document.Waits.First();
            var waitTarget = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, _eventType, EnumEventCorrespondentType.FromSourceToTarget);
            waitTarget.ParentId = waitParent.Id;
            waitTarget.OnEvent.SourcePositionId = waitParent.OnEvent.TargetPositionId;
            _document.Waits = new List<InternalDocumentWait> {  waitTarget };

            if (Model.SourcePositionId != waitParent.OnEvent.TargetPositionId)
            {
                _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model);
            }
            if (Model.IsAddControl)
            {
                ((List<InternalDocumentWait>)_document.Waits).AddRange(CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromSourceToSource));
            }

            Model.StartEvent = waitTarget.OnEvent;
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };

            _operationDb.SendBySendList(_context, _document);

            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), Model.SendType.ToString());
        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}