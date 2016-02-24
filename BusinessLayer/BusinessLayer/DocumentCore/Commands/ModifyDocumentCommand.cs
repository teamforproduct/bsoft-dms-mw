using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Commands
{
    internal class ModifyDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public ModifyDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
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
                return (ModifyDocument) _param;
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
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString()});
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.Description = Model.Description;
            _document.DocumentSubjectId = Model.DocumentSubjectId;
            _document.SenderAgentId = Model.SenderAgentId;
            _document.SenderAgentPersonId = Model.SenderAgentPersonId;
            _document.SenderNumber = Model.SenderNumber;
            _document.SenderDate = Model.SenderDate;
            _document.Addressee = Model.Addressee;
            _document.AccessLevel = Model.AccessLevel;

           CommonDocumentUtilities.VerifyDocument(_context, new FrontDocument(_document), null);
            _documentDb.ModifyDocument(_context, _document);
            return _document.Id;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.ModifyDocument; } }
    }
}