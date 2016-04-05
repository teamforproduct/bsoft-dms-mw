﻿using System;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель файла документа для отображения пользователю
    /// </summary>
    public class FrontDocumentAttachedFile
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FrontDocumentAttachedFile()
        {
        }

        /// <summary>
        /// конструктор для превращения внутренней модели файла в модель отображения для пользователя
        /// </summary>
        /// <param name="doc"></param>
        public FrontDocumentAttachedFile(InternalDocumentAttachedFile doc)
        {
            Id = doc.Id;
            DocumentId = doc.DocumentId;
            OrderInDocument = doc.OrderInDocument;

            Version = doc.Version;
            FileContent = doc.FileContent;
            Name = doc.Name;
            Extension = doc.Extension;

            FileType = doc.FileType;
            FileSize = doc.FileSize;
            IsAdditional = doc.IsAdditional;
            Date = doc.Date;
            Hash = doc.Hash;
            WasChangedExternal = doc.WasChangedExternal;
            LastChangeUserId = doc.LastChangeUserId;
            LastChangeDate = doc.LastChangeDate;
        }


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
        /// Размер файла
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// Хэш файла. для проверки целостности файла в хранилище
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Версия вложения
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Дата создания файла
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Признак, изменялся ли файл в хранищие извне
        /// </summary>
        public bool WasChangedExternal { get; set; }

        /// <summary>
        /// Имя пользователя, который последний редактировал файл
        /// </summary>
        public string LastChangeUserName { get; set; }

        /// <summary>
        /// ИД пользователя, последним изменившего запись
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата последнего изменения записи
        /// </summary>
        public DateTime LastChangeDate { get; set; }
    }
}