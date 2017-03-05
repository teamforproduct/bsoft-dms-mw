using System;
using System.Collections.Generic;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Model.DocumentCore.InternalModel
{
    /// <summary>
    /// Класс приатаченного к документу файла для внутреннего использования
    /// </summary>
    public class InternalDocumentAttachedFile : InternalTemplateAttachedFile
    {
        public InternalDocumentAttachedFile()
        {
        }

        public InternalDocumentAttachedFile(FrontDocumentAttachedFile doc)
        {
            Date = doc.Date;
            IsDeleted = doc.IsDeleted;
            IsMainVersion = doc.IsMainVersion;
            Version = doc.Version;
            Name = doc.Name;
            Extension = doc.Extension;
            Description = doc.Description;
            DocumentId = doc.DocumentId ?? -1;
            Id = doc.Id;
            FileType = doc.FileType;
            OrderInDocument = doc.OrderInDocument;
            FileContent = doc.FileContent;
        }

        /// <summary>
        /// Признак основная версия файла
        /// </summary>
        public bool IsMainVersion { get; set; }
        /// <summary>
        /// Признак удаленый файл или нет
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Признак принята ли версия файла
        /// </summary>
        public bool? IsWorkedOut { get; set; }
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

        public int? ExecutorPositionId { get; set; }
        public int ExecutorPositionExecutorAgentId { get; set; }
        public int? ExecutorPositionExecutorTypeId { get; set; }
        public IEnumerable<InternalDocumentEvent> Events { get; set; }
    }
}