using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.FileWorker;
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

        //public FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent)
        //{
        //    if (fileIdent.Version > 0)
        //    {
        //        return _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument,fileIdent.Version);
        //    }
        //    return GetDocumentFileLatestVersion(ctx, fileIdent);
        //}

        //public FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id)
        //{
        //    return _dbProcess.GetDocumentFileVersion(ctx, id);
        //}

        //public FrontDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, FilterDocumentFileIdentity fileIdent)
        //{
        //    return _dbProcess.GetDocumentFileLatestVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
        //}

        //public IEnumerable<FrontDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, FilterDocumentFileIdentity fileIdent)
        //{
        //    return _dbProcess.GetDocumentFileVersions(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
        //}

        //public void DeleteDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent)
        //{
        //    var fl = _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument, fileIdent.Version);
        //    if (fl != null)
        //    {
        //        _fStore.DeleteFileVersion(ctx, fl);
        //        _dbProcess.DeleteAttachedFile(ctx, fl);
        //    }
        //}

        //public FrontDocumentAttachedFile AddNewVersion(IContext ctx, ModifyDocumentFile model)
        //{
        //    //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
        //    var fl = _dbProcess.GetDocumentFileLatestVersion(ctx, model.DocumentId, model.OrderInDocument);
        //    if (fl != null)
        //    {
        //        var att = new FrontDocumentAttachedFile
        //        {
        //            DocumentId = model.DocumentId,
        //            Date = DateTime.Now,
        //            FileContent = Convert.FromBase64String(model.FileData),
        //            IsAdditional = fl.IsAdditional,
        //            LastChangeUserId = ctx.CurrentAgentId,
        //            LastChangeDate = DateTime.Now,
        //            Version = fl.Version + 1,
        //            FileType =  model.FileType,
        //            FileSize =  model.FileSize,
        //            OrderInDocument = model.OrderInDocument,
        //            Name = Path.GetFileNameWithoutExtension(model.FileName),
        //            Extension = Path.GetExtension(model.FileName).Replace(".", ""),
        //            WasChangedExternal = false
        //        };
        //        _fStore.SaveFile(ctx, att);
        //        _dbProcess.AddNewFileOrVersion(ctx, att);
        //        return att;
        //    }
        //    throw new UnknownDocumentFile();
        //}

    }
}