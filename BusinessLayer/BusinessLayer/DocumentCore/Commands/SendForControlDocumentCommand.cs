using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForControlDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForControlDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
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
                if (model.SendType != EnumSendTypes.SendForControl)
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
            DmsExceptions ex = null;
            if (_document == null)
            {
                ex = new DocumentNotFoundOrUserHasNoAccess();
            }
            else if (Model.StartEventId != null || Model.CloseEventId != null)
            {
                ex = new PlanPointHasAlredyBeenLaunched();
            }
            else if (!Model.TargetPositionId.HasValue)
            {
                ex = new TargetIsNotDefined();
            }
            else if (!Model.TaskId.HasValue)
            {
                ex = new TaskIsNotDefined();
            }
            else if (_document.Waits != null && _document.Waits.Any())
            {
                ex = new ResponsibleExecutorHasAlreadyBeenDefined();
            }
            else if (_document.Events != null && _document.Events.Any())
            {
                ex = new ControlerHasAlreadyBeenDefined();
            }
            if (Model.TargetPositionId.HasValue
                && !_admin.VerifySubordination(_context, new VerifySubordination
                {
                    SubordinationType = EnumSubordinationTypes.Execution,
                    TargetPosition = Model.TargetPositionId.Value,
                    SourcePositions = new List<int> { Model.SourcePositionId },
                }))
            {
                ex = new SubordinationHasBeenViolated();
            }
            if (ex != null)
            {
                Model.AddDescription = ex.Message;
                _operationDb.ModifyDocumentSendListAddDescription(_context, Model);
                throw ex;
            }
            CommonDocumentUtilities.PlanDocumentPaperFromSendList(_context, _document, Model);
            return true;
        }
        public override object Execute()
        {
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);
            //var waitTarget = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, _eventType, EnumEventCorrespondentType.FromTargetToTarget);
            //_document.Waits = new List<InternalDocumentWait> { waitTarget };

            //if (Model.SourcePositionId != Model.TargetPositionId)
            //{
            //    _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, Model);
            //}
            //Model.CloseEvent = Model.StartEvent = waitTarget.OnEvent;

            if (Model.IsAddControl)
            {
                ((List<InternalDocumentWait>)_document.Waits).AddRange(CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromSourceToSource));
            }

            Model.CloseEvent = Model.StartEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, Model);
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };

            _operationDb.SendBySendList(_context, _document);

            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), Model.SendType.ToString());

        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}