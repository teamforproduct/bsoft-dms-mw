using System;
using BL.CrossCutting.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentLinkCommand: BaseDocumentAdditionCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminsDbProcess _adminDb;

        protected InternalLinkedDocument DocLink;

        public AddDocumentLinkCommand(IDocumentOperationsDbProcess operationDb, IAdminsDbProcess adminDb)
        {
            _adminDb = adminDb;
            _operationDb = operationDb;
        }

        private AddDocumentLink Model
        {
            get
            {
                if (!(_param is AddDocumentLink))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentLink) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            DocLink = _operationDb.AddDocumentLinkPrepare(_context, Model);
            if (DocLink.DocumentId == null || DocLink.ParentDocumentId == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (DocLink.DocumentLinkId.HasValue && DocLink.ParentDocumentLinkId.HasValue && (DocLink.DocumentLinkId == DocLink.ParentDocumentLinkId))
            {
                throw new DocumentHasAlreadyHadLink();
            }
            _context.SetCurrentPosition(DocLink.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString()});
            return true;
        }

        public override object Execute()
        {
            DocLink.LastChangeUserId = _context.CurrentAgentId;
            DocLink.LastChangeDate = DateTime.Now;
            _operationDb.AddDocumentLink(_context, DocLink);
            return null;
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.AddDocumentLink;
    }
}