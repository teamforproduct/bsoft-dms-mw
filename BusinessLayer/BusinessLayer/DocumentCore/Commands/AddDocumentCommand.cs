using System;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    internal class AddDocumentCommand : BaseDocumentCommand
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
                return (AddDocumentByTemplateDocument) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;

        }

        public override bool CanExecute()
        {
            _adminDb.VerifyAccess(Context, CommandType);

            _document = _documentDb.AddDocumentPrepare(Context, Model.TemplateDocumentId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetAtrributesForNewDocument(Context, _document);

            foreach (var sl in _document.SendLists)
            {
                sl.IsInitial = true;
                sl.StartEventId = null;
                sl.CloseEventId = null;
                CommonDocumentUtilities.SetLastChange(Context, sl);
            }
            foreach (var sl in _document.RestrictedSendLists)
            {
                CommonDocumentUtilities.SetLastChange(Context, sl);
            }
            Document.Events = CommonDocumentUtilities.GetNewDocumentEvent(Context,EnumEventTypes.AddNewDocument, "Create");
            Document.Accesses = CommonDocumentUtilities.GetNewDocumentAccess(Context);
            //TODO process files
            Document.DocumentFiles = null;
            _documentDb.AddDocument(Context, Document);
            return Document.Id;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocument;
    }
}