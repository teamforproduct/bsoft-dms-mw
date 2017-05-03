using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.SystemServices.FileService;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

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

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null)
        {
            return _dbProcess.GetDocumentFiles(ctx, filter, paging);
        }

        private FrontDocumentAttachedFile GetUserFile(IContext ctx, int id, EnumDocumentFileType fileType)
        {
            var fl = _dbProcess.GetDocumentFileVersion(ctx, id);
            if (fl == null)
            {
                throw new UnknownDocumentFile();
            }
            if (fileType == EnumDocumentFileType.UserFile)
            {
                _fStore.GetFile(ctx, fl, fileType);
            }
            else
            {
                _fStore.GetFile(ctx, fl, fileType);
                var internalFile = new InternalDocumentAttachedFile { Id = fl.Id, LastPdfAccess = DateTime.Now, PdfCreated = true };
                _dbProcess.UpdateFilePdfView(ctx, internalFile);
            }

            return fl;
        }

        public FrontDocumentAttachedFile GetUserFile(IContext ctx, int id)
        {
            return GetUserFile(ctx, id, EnumDocumentFileType.UserFile);
        }

        public FrontDocumentAttachedFile GetUserFilePdf(IContext ctx, int id)
        {
            return GetUserFile(ctx, id, EnumDocumentFileType.PdfFile);
        }

        public FrontDocumentAttachedFile GetUserFilePreview(IContext ctx, int id)
        {
            return GetUserFile(ctx, id, EnumDocumentFileType.PdfPreview);
        }

    }
}