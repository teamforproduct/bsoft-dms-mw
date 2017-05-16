using System;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using System.ComponentModel;
using System.Threading.Tasks;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;

namespace BL.Logic.SystemServices.FileService
{
    public class FileService : IFileService
    {
        private readonly IDocumentFileService _fileService;

        public FileService(IDocumentFileService fileService)
        {
            _fileService = fileService;
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

        public string GetFileUri(string apiPrefix, string module, string feature, string fileType, int id)
        {
            return string.IsNullOrEmpty(fileType) ? $"/{apiPrefix}/{module}/{feature}/{id}" : $"/{apiPrefix}/{module}/{feature}/{fileType}/{id}";
        }

        // TODO Сервис получился завязан на файлы документов. Для шаблонов EnumFileTypes, аватарки пока хранятся в базе
        public Task<FrontDocumentFile> GetFile(IContext ctx, EnumDocumentFileType fileType, int id)
        {
            return Task.Factory.StartNew(() =>
            {
                FrontDocumentFile item;

                switch (fileType)
                {
                    case EnumDocumentFileType.UserFile:
                        item = _fileService.GetUserFile(ctx, id);
                        break;
                    case EnumDocumentFileType.PdfFile:
                        item = _fileService.GetUserFilePdf(ctx, id);
                        break;
                    case EnumDocumentFileType.PdfPreview:
                        item = _fileService.GetUserFilePreview(ctx, id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return item;
            });
        }
    }
}