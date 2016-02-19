using System;
using System.Collections.Generic;
using System.IO;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemLogic;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
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

        public IEnumerable<FrontFilterDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId)
        {
            return _dbProcess.GetDocumentFiles(ctx, documentId);
        }

        public IEnumerable<FrontFilterDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            return _dbProcess.GetDocumentFileVersions(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
        }

        public FrontFilterDocumentAttachedFile GetDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            if (fileIdent.Version > 0)
            {
                return _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument,
                    fileIdent.Version);
            }
            return GetDocumentFileLatestVersion(ctx, fileIdent);
        }

        public FrontFilterDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id)
        {
            return _dbProcess.GetDocumentFileVersion(ctx, id);
        }

        public FrontFilterDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            return _dbProcess.GetDocumentFileLatestVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
        }

        public void DeleteDocumentFile(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            var flList = _dbProcess.GetDocumentFileVersions(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument);
            foreach (var fl in flList)
            {
                _fStore.DeleteFile(ctx, fl);
                _dbProcess.DeleteAttachedFile(ctx, fl);
            }
        }

        public void DeleteDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            var fl = _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument, fileIdent.Version);
            if (fl != null)
            {
                _fStore.DeleteFile(ctx, fl);
                _dbProcess.DeleteAttachedFile(ctx, fl);
            }
        }

        public FrontFilterDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            //TODO should we check if file exists in DB? 
            var fl = GetDocumentFileVersion(ctx, fileIdent);
            _fStore.GetFile(ctx, fl);
            return fl;
        }

        public int AddUserFile(IContext ctx, ModifyDocumentFile model)
        {
            var att = new FrontFilterDocumentAttachedFile
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
        public IEnumerable<FrontFilterDocumentAttachedFile> AddUserFile(IContext ctx, ModifyDocumentFiles model)
        {
            var files = _dbProcess.GetDocumentFiles(ctx, model.DocumentId);

            var res = new List<FrontFilterDocumentAttachedFile>();
            foreach (var file in model.Files)
            {
                var att = new FrontFilterDocumentAttachedFile
                {
                    DocumentId = model.DocumentId,
                    Date = DateTime.Now,
                    FileContent = Convert.FromBase64String(file.FileData),
                    IsAdditional = file.IsAdditional,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Version = 1,
                    FileType = file.FileType,
                    OrderInDocument = _dbProcess.GetNextFileOrderNumber(ctx, model.DocumentId),
                    Name = Path.GetFileNameWithoutExtension(file.FileName),
                    Extension = Path.GetExtension(file.FileName).Replace(".", ""),
                    WasChangedExternal = false
                };
                _fStore.SaveFile(ctx, att);
                res.Add(_dbProcess.GetDocumentFileVersion(ctx,_dbProcess.AddNewFileOrVersion(ctx, att)));
            }

            return res;
        }

        public FrontFilterDocumentAttachedFile AddNewVersion(IContext ctx, ModifyDocumentFile model)
        {
            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            var fl = _dbProcess.GetDocumentFileLatestVersion(ctx, model.DocumentId, model.OrderInDocument);
            if (fl != null)
            {
                var att = new FrontFilterDocumentAttachedFile
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

        public FrontFilterDocumentAttachedFile UpdateCurrentFileVersion(IContext ctx, ModifyDocumentFile model)
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