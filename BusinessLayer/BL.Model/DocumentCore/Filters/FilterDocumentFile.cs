using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр файлов документов
    /// </summary>
    public class FilterDocumentFile
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
        /// <summary>
        /// Отобрать по связанным документам
        /// Работает только если в DocumentId передан один ID
        /// </summary>
        public bool AllLinkedDocuments { get; set; }
        /// <summary>
        /// Массив ИД файлов документов
        /// </summary>
        public List<int> FileId { get; set; }

        /// <summary>
        /// Отрывок названия по File
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Отрывок расширения по File
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Число "с" для отбора по размеру файла документа
        /// </summary>
        public int? SizeFrom { get; set; }
        /// <summary>
        /// Число "по" для отбора по размеру файла документа
        /// </summary>
        public int? SizeTo { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате создания файла документа
        /// </summary>
        public DateTime? CreateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате создания файла документа
        /// </summary>
        public DateTime? CreateToDate { get; set; }
        /// <summary>
        /// Массив ИД пользователей по файлу
        /// </summary>
        public List<int> AgentId { get; set; }

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
        /// Типы файлов: основной, дополнительный, ...
        /// </summary>
        public List<EnumFileTypes> Types { get; set; }
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
