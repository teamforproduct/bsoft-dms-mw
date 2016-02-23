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

        protected InternalLinkedDocument _docLink;

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
            _docLink = _operationDb.AddDocumentLinkPrepare(_context, Model);
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString()});
            if (_docLink.DocumentLinkId.HasValue && _docLink.ParentDocumentLinkId.HasValue && (_docLink.DocumentLinkId == _docLink.ParentDocumentLinkId))
            {
                throw new DocumentHasAlreadyHadLink();
            }

            return true;
        }

        public override object Execute()
        {
            _docLink.LastChangeUserId = _context.CurrentAgentId;
            _docLink.LastChangeDate = DateTime.Now;
            _operationDb.AddDocumentLink(_context, _docLink);
            return null;
        }

        public override EnumDocumentAdditionActions CommandType => EnumDocumentAdditionActions.AddDocumentLink;
    }
}