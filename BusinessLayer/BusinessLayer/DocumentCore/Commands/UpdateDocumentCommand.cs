using BL.Database.Documents.Interfaces;
using BL.CrossCutting.Common;
using BL.Database.Admins.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    internal class UpdateDocumentCommand : BaseCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public UpdateDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        private ModifyDocument Model
        {
            get
            {
                if (!(_param is ModifyDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return _param as ModifyDocument;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.ModifyDocumentPrepare(_context, Model.Id);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }

            _adminDb.VerifyAccess(_context, new VerifyAccess { ActionCode = EnumActions.ModifyDocument, PositionId = _document.ExecutorPositionId });
            return true;
        }

        public override object Execute()
        {
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _document.Description = Model.Description;
            _document.DocumentSubjectId = Model.DocumentSubjectId;
            _document.SenderAgentId = Model.SenderAgentId;
            _document.SenderAgentPersonId = Model.SenderAgentPersonId;
            _document.SenderNumber = Model.SenderNumber;
            _document.SenderDate = Model.SenderDate;
            _document.Addressee = Model.Addressee;
            _document.AccessLevel = Model.AccessLevel;

            CommonDocumentUtilities.SetLastChangeForDocument(_context, _document);

            CommonDocumentUtilities.VerifyDocument(_context, new FrontDocument(_document), null);
            _documentDb.UpdateDocument(_context, _document);
            return _document.Id;
        }
    }
}