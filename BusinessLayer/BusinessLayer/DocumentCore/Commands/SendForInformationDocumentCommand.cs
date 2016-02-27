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
                var model = (InternalDocumentSendList)_param;
                if (model.SendType != EnumSendTypes.SendForInformation && model.SendType != EnumSendTypes.SendForConsideration)
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
            _adminDb.VerifyAccess(_context, CommandType);
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
                _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);
            }
            var evt = CommonDocumentUtilities.GetNewDocumentEvent(_context, Model);
            Model.StartEvent = evt;
            Model.CloseEvent = evt;
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };
            _operationDb.SendForInformation(_context, _document);
            return null;
        }

        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}