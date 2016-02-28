using System;
using System.Collections.Generic;
using System.Linq;
using BL.Database.Admins.Interfaces;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForResponsibleExecutionDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public SendForResponsibleExecutionDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
            _adminDb = adminDb;
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
                if (model.SendType != EnumSendTypes.SendForResponsibleExecution)
                {
                    throw new WrongParameterTypeError();
                }
                return (InternalDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(Model.SourcePositionId);
            _adminDb.VerifyAccess(_context, CommandType);   //TODO без позиций
            _document = _documentDb.GetBlankInternalDocumentById(_context, Model.DocumentId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (Model.StartEventId != null || Model.CloseEventId != null)
            {
                throw new PlanPointHasAlredyBeenLaunched();
            }
            if (!Model.TargetPositionId.HasValue)
            {
                throw new WrongDocumentSendListEntry();
            }

            return true;
        }
        public override object Execute()
        {
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);

            var waitSource = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, EnumEventTypes.ControlOn,EnumEventCorrespondentType.FromSourceToSource);
            var waitTarget = CommonDocumentUtilities.GetNewDocumentWait(_context, Model, EnumEventTypes.SendForResponsibleExecution, EnumEventCorrespondentType.FromSourceToTarget);

            Model.StartEvent = waitSource.OnEvent;
            waitTarget.ParentWait = waitSource;

            CommonDocumentUtilities.SetLastChange(_context, Model);

            _document.SendLists = new List<InternalDocumentSendList> { Model };

            _document.Waits = new List<InternalDocumentWait> { waitSource, waitTarget };

            _operationDb.SendBySendList(_context, _document);

            return null;
        }

        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}