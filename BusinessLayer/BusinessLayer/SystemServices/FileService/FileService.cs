using System;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.SystemServices.FileService
{
    public class FileService : IFileService
    {
        private readonly IFileStore _fileStore;
        private readonly IDocumentFileDbProcess _dbProcess;

        public FileService(IFileStore fileStore, IDocumentFileDbProcess dbProcess)
        {
            _fileStore = fileStore;
            _dbProcess = dbProcess;
        }

        public string GetFileUri(string serverUrl, IContext ctx, EnumDocumentFileType fileType, int id)
        {
            return $"{serverUrl}files/{ctx.Client.Id}/{(int) fileType}/{id}";
        }

        public byte[] GetFile(IContext ctx, EnumDocumentFileType fileType, int id)
        {
            byte[] resFile =null;
            switch (fileType)
            {
                case EnumDocumentFileType.UserFile:
                case EnumDocumentFileType.PdfFile:
                case EnumDocumentFileType.PdfPreview:
                    
                    var fl = _dbProcess.GetDocumentFileVersion(ctx, id);
                    if (fl == null)
                    {
                        throw new UnknownDocumentFile();
                    }
                    if (fileType == EnumDocumentFileType.UserFile)
                    {
                        resFile = _fileStore.GetFile(ctx, fl, fileType);
                    }
                    else
                    {
                        resFile = _fileStore.GetFile(ctx, fl, fileType);
                        var internalFile = new InternalDocumentAttachedFile { Id = fl.Id, LastPdfAccess = DateTime.Now, PdfCreated = true };
                        _dbProcess.UpdateFilePdfView(ctx, internalFile);
                    }
                    break;
                case EnumDocumentFileType.Avatar:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }

            return resFile;
        }
    }
}