using System;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.ComponentModel;

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

        private string GetDescription(MimeTypes Band)
        {
            System.Reflection.FieldInfo oFieldInfo = Band.GetType().GetField(Band.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[]) oFieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return Band.ToString();
        }

        public string GetMimetype(string fileExt)
        {
            if (string.IsNullOrEmpty(fileExt)) throw new ArgumentException();
            try
            {
                var ext = fileExt.Replace(".", "").ToLower();
                var mimeType = (MimeTypes)Enum.Parse(typeof(MimeTypes), ext);

                string description = GetDescription(mimeType);
                return description;
            }
            catch 
            {
                return "application/octet-stream";
            }
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
                        var internalFile = new InternalDocumentFile { Id = fl.Id, LastPdfAccess = DateTime.Now, PdfCreated = true };
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