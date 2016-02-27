using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.SystemLogic;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    internal class AddDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;
        private readonly IFileStore _fStore;

        public AddDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb, IFileStore fStore)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
            _fStore = fStore;
        }

        private AddDocumentByTemplateDocument Model
        {
            get
            {
                if (!(_param is AddDocumentByTemplateDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentByTemplateDocument)_param;
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

            CommonDocumentUtilities.SetLastChange(Context, _document.RestrictedSendLists);

            Document.Events = CommonDocumentUtilities.GetNewDocumentEvents(Context, null, EnumEventTypes.AddNewDocument, "Create");
            Document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(Context);

            // prepare file list in Document. It will save it with document in DB
            var toCopy = new Dictionary<InternalDocumentAttachedFile, InternalTemplateAttachedFile>();
            int newOrdNum = 1;
            _document.DocumentFiles.ToList().ForEach(x =>
            {
                var fileToCopy = new InternalTemplateAttachedFile
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Extension = x.Extension,
                    Name = x.Name,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    OrderInDocument = x.OrderInDocument,
                    IsAdditional = x.IsAdditional,
                    Hash = x.Hash
                };
                var newDoc = new InternalDocumentAttachedFile
                {
                    Extension = x.Extension,
                    Name = x.Name,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    IsAdditional = x.IsAdditional,
                    OrderInDocument = newOrdNum,
                    Date = DateTime.Now,
                    Version = 1,
                    WasChangedExternal = false
                };
                newOrdNum++;
                toCopy.Add(newDoc, fileToCopy);
            });

            // assign new created list of files to document
            _document.DocumentFiles = toCopy.Keys;

            _documentDb.AddDocument(Context, Document);

            //after saving document in filelist it should be filled DocumentId field. So we can phisical copy files
            foreach (var fl in _document.DocumentFiles)
            {
                var dest = toCopy.Keys.FirstOrDefault(x => x.OrderInDocument == fl.OrderInDocument);
                var src = toCopy[dest];
                _fStore.CopyFile(_context, src, fl);
            }

            return Document.Id;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocument;
    }
}