﻿using BL.Model.Common;

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
        public bool IsAdditional { get; set; }
        /// <summary>
        /// содержимое файла
        /// </summary>
        public byte[] FileContent { get; set; }
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
        /// Хэш файла. для проверки целостности файла в хранилище
        /// </summary>
        public string Hash { get; set; }

    }
}