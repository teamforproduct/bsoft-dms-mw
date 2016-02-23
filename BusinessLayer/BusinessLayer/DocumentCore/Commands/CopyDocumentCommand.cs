using System;
using BL.CrossCutting.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class CopyDocumentCommand: BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public CopyDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        private CopyDocument Model
        {
            get
            {
                if (!(_param is CopyDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (CopyDocument) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            try
            {
                _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool CanExecute()
        {
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString()});
            _document = _documentDb.CopyDocumentPrepare(_context, Model.DocumentId);

            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetAtrributesForNewDocument(_context, _document);

            foreach (var sl in _document.SendLists)
            {
                sl.StartEventId = null;
                sl.CloseEventId = null;
                sl.LastChangeDate = DateTime.Now;
                sl.LastChangeUserId = _context.CurrentAgentId;
            }

            foreach (var sl in _document.RestrictedSendLists)
            {
                sl.LastChangeDate = DateTime.Now;
                sl.LastChangeUserId = _context.CurrentAgentId;
            }

            _document.Events = CommonDocumentUtilities.GetEventForNewDocument(_context);
            _document.Accesses = CommonDocumentUtilities.GetAccessesForNewDocument(_context);

            //TODO process files
            _document.DocumentFiles = null;

            //TODO make it with Actions
            _documentDb.AddDocument(_context, _document);
            return _document.Id;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.CopyDocument; } }
    }
}