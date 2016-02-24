using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.Commands
{
    internal class DeleteDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public DeleteDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            return !_document.IsRegistered;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.DeleteDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, CommandType);

           if (!CanBeDisplayed())
            {
                throw new DocumentCannotBeModifiedOrDeleted();
            }
            return true;
        }

        public override object Execute()
        {
            _documentDb.DeleteDocument(_context, _document.Id);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.DeleteDocument;
    }
}