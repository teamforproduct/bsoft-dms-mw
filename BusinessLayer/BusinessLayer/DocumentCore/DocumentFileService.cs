using System;
using System.Collections.Generic;
using System.IO;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents;
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

        public IEnumerable<DocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber)
        {
            return _dbProcess.GetDocumentFileVersions(ctx, documentId, orderNumber);
        }

        public DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber)
        {
            return GetDocumentFileVersion(ctx, documentId, orderNumber, versionNumber);
        }

        public DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id)
        {
            return _dbProcess.GetDocumentFileVersion(ctx, id);
        }

        public DocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber)
        {
            return _dbProcess.GetDocumentFileLatestVersion(ctx, documentId, orderNumber);
        }

        public void DeleteDocumentFile(IContext ctx, int documentId, int ordNumber)
        {
            var flList = _dbProcess.GetDocumentFileVersions(ctx, documentId, ordNumber);
            foreach (var fl in flList)
            {
                _fStore.DeleteFile(ctx, fl);
                _dbProcess.DeleteAttachedFile(ctx, fl);
            }
        }

        public void DeleteDocumentFileVersion(IContext ctx, int documentId, int ordNumber, int versionId)
        {
            var fl = _dbProcess.GetDocumentFileVersion(ctx, documentId, ordNumber, versionId);
            if (fl != null)
            {
                _fStore.DeleteFile(ctx, fl);
                _dbProcess.DeleteAttachedFile(ctx, fl);
            }
        }

        public byte[] GetUserFile(IContext ctx, DocumentAttachedFile attFile)
        {
            //TODO should we check if file exists in DB? 
            return _fStore.GetFile(ctx, attFile);
        }

        public int AddUserFile(IContext ctx, int documentId, string fileName, byte[] fileData, bool isAdditional)
        {
            var att = new DocumentAttachedFile
            {
                DocumentId = documentId,
                Date = DateTime.Now,
                FileData = fileData,
                IsAdditional   = isAdditional,
                LastChangeUserId = ctx.CurrentAgentId,
                LastChangeDate = DateTime.Now,
                Version = 1,
                OrderInDocument = _dbProcess.GetNextFileOrderNumber(ctx, documentId),
                Name = Path.GetFileNameWithoutExtension(fileName),
                Extension = Path.GetExtension(fileName).Replace(".",""),
                WasChangedExternal = false
            };
            _fStore.SaveFile(ctx, att);
            return _dbProcess.AddNewFileOrVersion(ctx, att);
        }

        public int AddNewVersion(IContext ctx, int documentId, int fileOrder, string fileName, byte[] fileData)
        {
            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            var fl = _dbProcess.GetDocumentFileLatestVersion(ctx, documentId, fileOrder);
            if (fl != null)
            {
                var att = new DocumentAttachedFile
                {
                    DocumentId = documentId,
                    Date = DateTime.Now,
                    FileData = fileData,
                    IsAdditional = fl.IsAdditional,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now,
                    Version = fl.Version + 1,
                    OrderInDocument = fileOrder,
                    Name = Path.GetFileNameWithoutExtension(fileName),
                    Extension = Path.GetExtension(fileName).Replace(".", ""),
                    WasChangedExternal = false
                };
                _fStore.SaveFile(ctx, att);
                return _dbProcess.AddNewFileOrVersion(ctx, att);
            }
            throw new UnknownDocumentFile();
        }

        public bool UpdateCurrentFileVersion(IContext ctx, int documentId, int fileOrder, string fileName, byte[] fileData, int version = 0)
        {
            //TODO potential two user could add same new version in same time. Probably need to implement CheckOut flag in future
            var fl = _dbProcess.GetDocumentFileLatestVersion(ctx, documentId, fileOrder);
            if (fl != null)
            {
                _fStore.SaveFile(ctx, fl);
                fl.LastChangeDate = DateTime.Now;
                fl.LastChangeUserId = ctx.CurrentAgentId;
                _dbProcess.UpdateFileOrVersion(ctx,fl);
                return true;
            }
            throw new UnknownDocumentFile();
        }
    }
}