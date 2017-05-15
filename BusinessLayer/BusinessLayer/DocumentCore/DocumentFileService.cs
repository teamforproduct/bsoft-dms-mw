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
            var items = (List<FrontDocumentFile>)_dbProcess.GetDocumentFiles(ctx, filter, paging);

            var fileService = DmsResolver.Current.Get<IFileService>();

            items.ForEach(x =>
            {
                x.FileLink = fileService.GetFileUri(EnumDocumentFileType.UserFile, x.Id);
                x.PdfFileLink = fileService.GetFileUri(EnumDocumentFileType.PdfFile, x.Id);
                x.PreviewFileLink = fileService.GetFileUri(EnumDocumentFileType.PdfPreview, x.Id);
            });

            return items;
        }

        private FrontDocumentFile GetUserFile(IContext ctx, int id, EnumDocumentFileType fileType)
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
                var internalFile = new InternalDocumentFile { Id = fl.Id, LastPdfAccess = DateTime.Now, PdfCreated = true, File = fl.File };
                _dbProcess.UpdateFilePdfView(ctx, internalFile);
            }

            return fl;
        }

        public FrontDocumentFile GetUserFile(IContext ctx, int id)
        {
            return GetUserFile(ctx, id, EnumDocumentFileType.UserFile);
        }

        public FrontDocumentFile GetUserFilePdf(IContext ctx, int id)
        {
            return GetUserFile(ctx, id, EnumDocumentFileType.PdfFile);
        }

        public FrontDocumentFile GetUserFilePreview(IContext ctx, int id)
        {
            return GetUserFile(ctx, id, EnumDocumentFileType.PdfPreview);
        }

    }
}