using System;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    /// <summary>
    /// Класс приатаченного к документу файла для внутреннего использования
    /// </summary>
    public class InternalDocumentAttachedFile :  LastChangeInfo
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
        /// Версия вложения
        /// </summary>
        public int Version { get; set; }

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
        /// Признак дополнительный файл или основной
        /// </summary>
        public bool IsAdditional { get; set; }
        /// <summary>
        /// Дата создания файла
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Хэш файла. для проверки целостности файла в хранилище
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// Признак, изменялся ли файл в хранищие извне
        /// </summary>
        public bool WasChangedExternal { get; set; }
    }
}