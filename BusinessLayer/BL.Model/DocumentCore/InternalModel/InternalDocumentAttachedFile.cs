using System;
using BL.Model.DocumentCore.FrontModel;


namespace BL.Model.DocumentCore.InternalModel
{
    /// <summary>
    /// Класс приатаченного к документу файла для внутреннего использования
    /// </summary>
    public class InternalDocumentAttachedFile : InternalTemplateAttachedFile
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public InternalDocumentAttachedFile()
        {
        }

        /// <summary>
        /// create internal document attached file based on front model
        /// </summary>
        /// <param name="doc"></param>
        public InternalDocumentAttachedFile(FrontDocumentAttachedFile doc)
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
        }

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
    }
}