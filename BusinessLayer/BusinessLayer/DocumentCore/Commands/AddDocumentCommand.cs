using System;
using BL.CrossCutting.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    internal class AddDocumentCommand : BaseCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public AddDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
        }

        private AddDocumentByTemplateDocument Model {
            get
            {
                if (!(_param is AddDocumentByTemplateDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return _param as AddDocumentByTemplateDocument;
            }
        }

        public override bool CanBeDisplayed()
        {
            try
            {
                _adminDb.VerifyAccess(_context,new VerifyAccess {ActionCode = EnumActions.AddDocument, PositionId = _context.CurrentPositionId});
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public override bool CanExecute()
        {
            _adminDb.VerifyAccess(_context, new VerifyAccess { ActionCode = EnumActions.AddDocument, PositionId = _context.CurrentPositionId });

            _document = _documentDb.AddDocumentByTemplateDocumentPrepare(_context, Model.TemplateDocumentId);
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
                sl.IsInitial = true;
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
            _documentDb.AddDocument(_context, _document);
            return _document.Id;
        }
    }
}