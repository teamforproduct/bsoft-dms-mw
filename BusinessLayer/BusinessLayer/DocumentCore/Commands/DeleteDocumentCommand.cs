using BL.Database.Documents.Interfaces;
using BL.Model.Exception;
using BL.CrossCutting.Common;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore.Commands
{
    internal class DeleteDocumentCommand : BaseCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public DeleteDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        public override bool CanBeDisplayed()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            return !_document.IsRegistered;
        }

        public override object Execute()
        {
            _documentDb.DeleteDocument(_context, _document.Id);
            return null;
        }

        public override bool CanExecute()
        {
            if (!CanBeDisplayed())
            {
                throw new DocumentCannotBeModifiedOrDeleted();
            }
            _adminDb.VerifyAccess(_context, new VerifyAccess { ActionCode = EnumActions.DeleteDocument, PositionId = _document.ExecutorPositionId });

            return true;
        }
    }
}