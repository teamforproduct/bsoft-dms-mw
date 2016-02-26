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
    public class SendForInformationDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        public SendForInformationDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
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
                return (InternalDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.GetBlankInternalDocumentById(_context, Model.DocumentId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (Model.StartEventId != null || Model.CloseEventId != null)
            {
                throw new PlanPointHasAlredyBeenLaunched();
            }
            return true;
        }
        public override object Execute()
        {
            if (Model.TargetPositionId.HasValue)
            {
                _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccess(_context, Model.AccessLevel, Model.TargetPositionId.Value);
            }
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvent(_context, Model.DocumentId, _eventType, Model.Description);
            Model.StartEvent = _document.Events.First();
            Model.CloseEvent = _document.Events.First();
            CommonDocumentUtilities.SetLastChange(_context,Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };
            _operationDb.SendForInformation(_context, _document);
            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}