﻿using System;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using System.ComponentModel;
using System.Threading.Tasks;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.FrontModel;

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
                (DescriptionAttribute[])oFieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

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
            var res = $"/{apiPrefix.TrimEnd('/')}/{module}/{feature}/{id}";

            if (!string.IsNullOrEmpty(fileType)) res = $"{res}/{fileType}";

            return res;
        }

        // TODO Сервис получился завязан на файлы документов. Для шаблонов EnumFileTypes, аватарки пока хранятся в базе
        public Task<FrontDocumentFile> GetFile(IContext ctx, EnumDocumentFileType fileType, int id)
        {
            return Task.Factory.StartNew(() =>
            {
                FrontDocumentFile item;
                item = _fileService.GetDocumentFile(ctx, id, fileType);
                return item;
            });
        }
    }
}