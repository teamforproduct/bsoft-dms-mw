using System;
using System.Collections.Generic;
using System.IO;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemLogic;
using BL.Model.DocumentAdditional;
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

        public IEnumerable<DocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId)
        {
            return _dbProcess.GetDocumentFiles(ctx, documentId);
        }

        public IEnumerable<DocumentAttachedFile> GetDocumentFileVersions(IContext ctx, DocumentFileIdentity fileIdent)
        {
            return _dbProcess.GetDocumentFileVersions(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
        }

        public DocumentAttachedFile GetDocumentFileVersion(IContext ctx, DocumentFileIdentity fileIdent)
        {
            if (fileIdent.Version > 0)
            {
                return _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument,
                    fileIdent.Version);
            }
            return GetDocumentFileLatestVersion(ctx, fileIdent);
        }

        public DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id)
        {
            return _dbProcess.GetDocumentFileVersion(ctx, id);
        }

        public DocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, DocumentFileIdentity fileIdent)
        {
            return _dbProcess.GetDocumentFileLatestVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
        }

        public void DeleteDocumentFile(IContext ctx, DocumentFileIdentity fileIdent)
        {
            var flList = _dbProcess.GetDocumentFileVersions(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
            foreach (var fl in flList)
            {
                _fStore.DeleteFile(ctx, fl);
                _dbProcess.DeleteAttachedFile(ctx, fl);
            }
        }

        public void DeleteDocumentFileVersion(IContext ctx, DocumentFileIdentity fileIdent)
        {
            var fl = _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument, fileIdent.Version);
            if (fl != null)
            {
                _fStore.DeleteFile(ctx, fl);
                _dbProcess.DeleteAttachedFile(ctx, fl);
            }
        }

        public DocumentAttachedFile GetUserFile(IContext ctx, DocumentFileIdentity fileIdent)
        {
            //TODO should we check if file exists in DB? 
            var fl = GetDocumentFileVersion(ctx, fileIdent);
            _fStore.GetFile(ctx, fl);
            return fl;
        }

        public int AddUserFile(IContext ctx, ModifyDocumentFile model)
        {
            var att = new DocumentAttachedFile
            {
                DocumentId = model.DocumentId,
                Date = DateTime.Now,
                FileContent = Convert.FromBase64String(model.FileData),
                IsAdditional = model.IsAdditional,
                LastChangeUserId = ctx.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Version = 1,
                FileType = model.FileType,
                OrderInDocument = _dbProcess.GetNextFileOrderNumber(ctx, model.DocumentId),
                Name = Path.GetFileNameWithoutExtension(model.FileName),
                Extension = Path.GetExtension(model.FileName).Replace(".", ""),
                WasChangedExternal = false
            };
            _fStore.SaveFile(ctx, att);
            return _dbProcess.AddNewFileOrVersion(ctx, att);
        }

        public DocumentAttachedFile AddNewVersion(IContext ctx, ModifyDocumentFile model)
        {
            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            var fl = _dbProcess.GetDocumentFileLatestVersion(ctx, model.DocumentId, model.OrderInDocument);
            if (fl != null)
            {
                var att = new DocumentAttachedFile
                {
                    DocumentId = model.DocumentId,
                    Date = DateTime.Now,
                    FileContent = Convert.FromBase64String(model.FileData),
                    IsAdditional = fl.IsAdditional,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Version = fl.Version + 1,
                    FileType =  model.FileType,
                    OrderInDocument = model.OrderInDocument,
                    Name = Path.GetFileNameWithoutExtension(model.FileName),
                    Extension = Path.GetExtension(model.FileName).Replace(".", ""),
                    WasChangedExternal = false
                };
                _fStore.SaveFile(ctx, att);
                _dbProcess.AddNewFileOrVersion(ctx, att);
                return att;
            }
            throw new UnknownDocumentFile();
        }

        public DocumentAttachedFile UpdateCurrentFileVersion(IContext ctx, ModifyDocumentFile model)
        {
            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            var fl = _dbProcess.GetDocumentFileLatestVersion(ctx, model.DocumentId, model.OrderInDocument);
            if (fl != null)
            {
                fl.FileContent = Convert.FromBase64String(model.FileData);
                fl.FileType = model.FileType;
                fl.Extension = Path.GetExtension(model.FileName).Replace(".", "");
                fl.Name = Path.GetFileNameWithoutExtension(model.FileName);
                fl.IsAdditional = fl.IsAdditional;
                fl.Date = DateTime.Now;
                _fStore.SaveFile(ctx, fl);
                fl.LastChangeDate = DateTime.Now;
                fl.LastChangeUserId = ctx.CurrentAgentId;
                _dbProcess.UpdateFileOrVersion(ctx,fl);
                return fl;
            }
            throw new UnknownDocumentFile();
        }
    }
}