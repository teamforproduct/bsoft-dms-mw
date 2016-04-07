using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore
{
    public class DocumentFileService : IDocumentFileService
    {
        private readonly IFileStore _fStore;
        private readonly IDocumentFileDbProcess _dbProcess;

        public DocumentFileService(IFileStore fStore, IDocumentFileDbProcess dbProcess)
        {
            _fStore = fStore;
            _dbProcess = dbProcess;
        }

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId)
        {
            return _dbProcess.GetDocumentFiles(ctx, documentId);
        }

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterDocumentAttachedFile filter)
        {
            return _dbProcess.GetDocumentFiles(ctx, filter);
        }

        public FrontDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            var fl = _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument, 1);
            if (fl == null)
            {
                throw new UnknownDocumentFile();
            }
            _fStore.GetFile(ctx, fl);
            return fl;
        }

    }
}