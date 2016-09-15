﻿using BL.Model.Common;
using BL.Model.Enums;
using System.Web;

namespace BL.Model.DocumentCore.InternalModel
{
    /// <summary>
    /// Внутренний класс для работы с файлами, прикрепленными к шаблону документа
    /// </summary>
    public class InternalTemplateAttachedFile : LastChangeInfo
    {
        /// <summary>
        /// ИД.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Номер вложения в документе
        /// </summary>
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Признак дополнительный файл или основной
        /// </summary>
        public EnumFileTypes Type { get; set; }
        /// <summary>
        /// Признак дополнительный файл или основной
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// содержимое файла
        /// </summary>
        public string FileContent { get; set; }
        /// <summary>
        /// содержимое файла
        /// </summary>
        public HttpPostedFile PostedFileData { get; set; }
        /// <summary>
        /// содержимое файла
        /// </summary>
        public byte[] FileData { get; set; }
        /// <summary>
        /// Название файла без расширения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Тип файла
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Размер файла
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Хэш файла. для проверки целостности файла в хранилище
        /// </summary>
        public string Hash { get; set; }
    }
}