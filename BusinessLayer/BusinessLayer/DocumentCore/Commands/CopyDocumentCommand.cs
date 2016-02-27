using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.SystemLogic;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class CopyDocumentCommand: BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;
        private readonly IFileStore _fStore;

        public CopyDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb, IFileStore fStore)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
            _fStore = fStore;
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
            _adminDb.VerifyAccess(_context, CommandType);
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
                CommonDocumentUtilities.SetLastChange(_context, sl);
            }

            CommonDocumentUtilities.SetLastChange(_context, _document.RestrictedSendLists);

            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context,null, EnumEventTypes.AddNewDocument, "Copy");
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context);

            // prepare file list in Document. It will save it with document in DB
            var toCopy = new Dictionary<InternalDocumentAttachedFile, InternalDocumentAttachedFile>();
            _document.DocumentFiles.ToList().ForEach(x =>
            {
                var newDoc = new InternalDocumentAttachedFile
                {
                    Extension = x.Extension,
                    Name = x.Name,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    IsAdditional = x.IsAdditional,
                    OrderInDocument = x.OrderInDocument,
                    Date = DateTime.Now,
                    Version = x.Version,
                    WasChangedExternal = false
                };
                toCopy.Add(newDoc, x);
            });

            // assign new created list of files to document
            _document.DocumentFiles = toCopy.Keys;

            _documentDb.AddDocument(_context, _document);

            //after saving document in filelist it should be filled DocumentId field. So we can phisical copy files
            foreach (var fl in _document.DocumentFiles)
            {
                var dest = toCopy.Keys.FirstOrDefault(x => x.OrderInDocument == fl.OrderInDocument);
                var src = toCopy[dest];
                _fStore.CopyFile(_context, src, fl);
            }

            return _document.Id;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.CopyDocument;
    }
}