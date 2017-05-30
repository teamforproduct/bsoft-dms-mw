using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Logic.SystemServices.FileService;
using BL.CrossCutting.DependencyInjection;

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

        public IEnumerable<FrontDocumentFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null)
        {
            return _dbProcess.GetDocumentFiles(ctx, filter, paging);
        }

        private FrontDocumentFile GetDocumentFile(IContext ctx, int id, EnumDocumentFileType fileType)
        {
            var fl = _dbProcess.GetDocumentFile(ctx, id);
            if (fl == null)
            {
                throw new UnknownDocumentFile();
            }
            if (!fl.IsDeleted)
                if (fileType == EnumDocumentFileType.UserFile)
                {
                    _fStore.GetFile(ctx, fl, fileType);
                }
                else
                {
                    _fStore.GetFile(ctx, fl, fileType);
                    var internalFile = new InternalDocumentFile { Id = fl.Id, LastPdfAccess = DateTime.Now, PdfCreated = true, PdfAcceptable = true, File = fl.File };
                    _dbProcess.UpdateFilePdfView(ctx, internalFile);
                }

            return fl;
        }

        public FrontDocumentFile GetUserFile(IContext ctx, int id)
        {
            return GetDocumentFile(ctx, id, EnumDocumentFileType.UserFile);
        }

        public FrontDocumentFile GetUserFilePdf(IContext ctx, int id)
        {
            return GetDocumentFile(ctx, id, EnumDocumentFileType.PdfFile);
        }

        public FrontDocumentFile GetUserFilePreview(IContext ctx, int id)
        {
            return GetDocumentFile(ctx, id, EnumDocumentFileType.PdfPreview);
        }

        public void DeleteDocumentFileFinal(IContext ctx)
        {
            var days = DmsResolver.Current.Get<ISettingValues>().GetClearTrashFilesTimeoutDayForClear(ctx);
            _dbProcess.DeleteDocumentFileFinal(ctx, days);
        }

    }
}