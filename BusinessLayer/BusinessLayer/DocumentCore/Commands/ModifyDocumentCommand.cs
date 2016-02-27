using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Commands
{
    internal class ModifyDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminService _admin;

        public ModifyDocumentCommand(IDocumentsDbProcess documentDb, IAdminService admin)
        {
            _documentDb = documentDb;
            _admin = admin;
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
            _document = _documentDb.ModifyDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);
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
            if (_document.Accesses != null)
            {
                var docAcc = _document.Accesses.First();
                CommonDocumentUtilities.SetLastChange(_context, docAcc);
                docAcc.AccessLevel = Model.AccessLevel;
            }
            CommonDocumentUtilities.VerifyDocument(_context, new FrontDocument(_document), null);    //TODO отвязаться от фронт-модели

            _documentDb.ModifyDocument(_context, _document);
            return _document.Id;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocument;
    }
}