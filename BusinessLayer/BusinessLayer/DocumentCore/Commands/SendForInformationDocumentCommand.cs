using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForInformationDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForInformationDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
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
                if (model.SendType != EnumSendTypes.SendForInformationExternal && model.SendType != EnumSendTypes.SendForInformation && model.SendType != EnumSendTypes.SendForConsideration)
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
            _adminProc.VerifyAccess(_context, CommandType);   //TODO без позиций
            _document = _operationDb.SendForInformationDocumentPrepare(_context, Model);
            DmsExceptions ex = null;
            if (_document == null)
            {
                ex = new DocumentNotFoundOrUserHasNoAccess();
            }
            else if (Model.StartEventId != null || Model.CloseEventId != null)
            {
                ex = new PlanPointHasAlredyBeenLaunched();
            }
            if (ex != null) CommonDocumentUtilities.ThrowError(_context, ex, Model);
            CommonDocumentUtilities.PlanDocumentPaperFromSendList(_context, _document, Model);
            return true;
        }
        public override object Execute()
        {
            var newEvent = Model.CloseEvent = Model.StartEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, Model);
            var ex = CommonDocumentUtilities.VerifyAndSetDocumentAccess(_context, _document, newEvent,
                new VerifySubordination
                {
                    SubordinationType = EnumSubordinationTypes.Informing,
                    TargetPosition = newEvent.Accesses.Where(x => x.AccessType != EnumEventAccessTypes.Source && x.PositionId.HasValue).Select(x => x.PositionId.Value).ToList(),
                    SourcePositions = CommonDocumentUtilities.GetSourcePositionsForSubordinationVeification(_context, Model, _document),
                },
                false, Model.AccessLevel);
            if (ex != null) CommonDocumentUtilities.ThrowError(_context, ex, Model);
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.Subscriptions = null;
            _document.SendLists = new List<InternalDocumentSendList> { Model };

            if (Model.IsAddControl)
            {
                _document.Waits = CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromSourceToSource, false);
            }

            _operationDb.SendBySendList(_context, _document);

            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), Model.SendType.ToString());

        public override EnumActions CommandType => (EnumActions)Enum.Parse(typeof(EnumActions), Model.SendType.ToString());
    }
}