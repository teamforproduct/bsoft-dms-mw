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
        /// Массив порядковых номеров файлов в списке файлов документа
        /// </summary>
        public List<int> OrderInDocument { get; set; }

        /// <summary>
        /// true - Получить только обработаные файлы
        /// false - Получить только не обработаные файлы
        /// null - Не применять фильтер
        /// По умолчанию null
        /// </summary>
        public bool? IsWorkedOut { get; set; }

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
        /// По умолчанию false
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// true - Игнорировать фильтр IsDeleted
        /// false - Применять фильтр IsDeleted
        /// По умолчанию false
        /// </summary>
        public bool IsAllDeleted { get; set; }
        /// <summary>
        /// true - Получить только основные версии файлов
        /// false - Получить не основные версии файлов
        /// По умолчанию true
        /// </summary>
        public bool IsMainVersion { get; set; } = true;
        /// <summary>
        /// true - Игнорировать фильтр IsMainVersion
        /// false - Применять фильтр IsMainVersion
        /// По умолчанию false
        /// </summary>
        public bool IsAllVersion { get; set; }

    }
}
