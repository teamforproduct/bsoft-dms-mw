using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр событий документов
    /// </summary>
    public class FilterDocumentAttachedFile
    {
        /// <summary>
        /// Массив ИД файлов документов
        /// </summary>
        public List<int> AttachedFileId { get; set; }
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }

        /// <summary>
        /// true - Получить только дополнительные файлы
        /// false - Получить только не дополнительные файлы
        /// null - Не применять фильтер
        /// По умолчанию null
        /// </summary>
        public bool? IsAdditional { get; set; }
        /// <summary>
        /// true - Получить только удаленные файлы
        /// false - Получить только не удаленные файлы
        /// null - Не применять фильтер
        /// По умолчанию false
        /// </summary>
        public bool? IsDeleted { get; set; } = false;
        /// <summary>
        /// true - Получить только основные версии файлов
        /// false - Получить не основные версии файлов
        /// null - Не применять фильтер
        /// По умолчанию true
        /// </summary>
        public bool? IsMainVersion { get; set; } = true;
        /// <summary>
        /// true - Получить только последние версии файлов
        /// false - Получить только не последние версии файлов
        /// null - Не применять фильтер
        /// По умолчанию null
        /// </summary>
        public bool? IsLastVersion { get; set; }
    }
}
