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
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore
{
    public class DocumentFileService : IDocumentFileService
    {
        private readonly IFileStore _fStore;
        private readonly IDocumentFileDbProcess _dbProcess;
        private readonly IAdminService _adminService;

        public DocumentFileService(IFileStore fStore, IDocumentFileDbProcess dbProcess, IAdminService adminService)
        {
            _fStore = fStore;
            _dbProcess = dbProcess;
            _adminService = adminService;
        }

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null)
        {
            _adminService.VerifyAccess(ctx, EnumDocumentActions.ViewDocument, false);
            return _dbProcess.GetDocumentFiles(ctx, filter, paging);

        }

        private FrontDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent, EnumDocumentFileType fileType)
        {
            var fl = _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument, fileIdent.Version ?? 0);
            if (fl == null)
            {
                throw new UnknownDocumentFile();
            }
            if (fileType != EnumDocumentFileType.UserFile)
            {
                _fStore.GetFile(ctx, fl, fileType);
            }
            else
            {
                if (!fl.PdfCreated) throw new UserPdfFileNotExists();
                _fStore.GetFile(ctx, fl, fileType);
                var internalFile = new InternalDocumentAttachedFile { Id = fl.Id, LastPdfAccess = DateTime.Now, PdfCreated = fl.PdfCreated };
                _dbProcess.UpdateFilePdfView(ctx, internalFile);
            }

            return fl;
        }

        public FrontDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            return GetUserFile(ctx, fileIdent, EnumDocumentFileType.UserFile);
        }

        public FrontDocumentAttachedFile GetUserFilePdf(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            return GetUserFile(ctx, fileIdent, EnumDocumentFileType.PdfFile);
        }

        public FrontDocumentAttachedFile GetUserFilePreview(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            return GetUserFile(ctx, fileIdent, EnumDocumentFileType.PdfPreview);
        }

    }
}