using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.FileWorker;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Commands
{
    public class CopyDocumentCommand: BaseDocumentCommand
    {

        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminService _admin;
        private readonly IFileStore _fStore;

        public CopyDocumentCommand(IDocumentsDbProcess documentDb, IAdminService admin, IFileStore fStore)
        {
            _documentDb = documentDb;
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
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
                sl.SourceAgentId = _context.CurrentAgentId;
                CommonDocumentUtilities.SetLastChange(_context, sl);
            }

            CommonDocumentUtilities.SetLastChange(_context, _document.RestrictedSendLists);

            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context,null, EnumEventTypes.AddNewDocument, "Copy");
            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context);

            // prepare file list in Document. It will save it with document in DB
            var toCopy = new Dictionary<InternalDocumentAttachedFile, InternalDocumentAttachedFile>();
            _document.DocumentFiles.ToList().ForEach(x =>
            {
                var newDoc = CommonDocumentUtilities.GetNewDocumentAttachedFile(x);
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

    }
}