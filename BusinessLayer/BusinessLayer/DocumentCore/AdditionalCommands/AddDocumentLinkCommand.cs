using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Model.SystemCore;


namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentLinkCommand: BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IAdminService _admin;

        public AddDocumentLinkCommand(IDocumentOperationsDbProcess operationDb, IAdminService admin)
        {
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId, InternalSystemAction action)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.AddDocumentLinkPrepare(_context, Model);
            if (_document?.Id == null || _document?.ParentDocumentId == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.LinkId.HasValue && _document.ParentDocumentLinkId.HasValue && (_document.LinkId == _document.ParentDocumentLinkId))
            {
                throw new DocumentHasAlreadyHadLink();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _operationDb.AddDocumentLink(_context, _document);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentLink;
    }
}