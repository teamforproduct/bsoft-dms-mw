using BL.Model.Enums;
using BL.Model.Extensions;
using BL.Model.SystemCore.Filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр событий документов
    /// </summary>
    public class FilterDocumentEvent
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
        /// Массив ИД событий документов
        /// </summary>
        public List<int> EventId { get; set; }

        /// <summary>
        /// Отбор по событиям признаку прочтения Все/Только новые
        /// </summary>
        public bool? IsNew { get; set; }
        /// <summary>
        /// Отбор по событиям, да - одно субъектны, нет - двусубъектные, нул - все
        /// </summary>
        public bool? IsSingleSubject { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате событий документа
        /// </summary>
        public DateTime? FromDate { get { return _FromDate; } set { _FromDate = value.ToUTC(); } }
        private DateTime? _FromDate;
        /// <summary>
        /// Дата "по" для отбора по дате событий документа
        /// </summary>
        public DateTime? ToDate { get { return _ToDate; } set { _ToDate = value.ToUTC(); } }
        private DateTime? _ToDate;
        /// <summary>
        /// Массив ИД типов событий
        /// </summary>
        public List<EnumEventTypes> TypeId { get; set; }
        /// <summary>
        /// Массив ИД важности событий
        /// </summary>
        public List<EnumImportanceEventTypes> ImportanceEventTypeId { get; set; }
        /// <summary>
        /// Отрывок краткого содержания по событиям
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Массив ИД должностей отправителей или получателей событий по документу
        /// </summary>
        public List<int> PositionId { get; set; }
        /// <summary>
        /// Массив ИД агентов внутрених отправителей или получателей событий по документу
        /// </summary>
        public List<int> PositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД  департаментов отправителей или получателей событий по документу
        /// </summary>
        public List<int> DepartmentId { get; set; }
        /// <summary>
        /// Массив ИД агентов внутрених или внешних отправителей или получателей событий по документу
        /// </summary>
        public List<int> AgentId { get; set; }

        /// <summary>
        /// Массив ИД должностей отправителей событий по документу
        /// </summary>
        public List<int> SourcePositionId { get; set; }
        /// <summary>
        /// Массив ИД агентов внутрених отправителей событий по документу
        /// </summary>
        public List<int> SourcePositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД  департаментов отправителей событий по документу
        /// </summary>
        public List<int> SourceDepartmentId { get; set; }
        /// <summary>
        /// Массив ИД агентов внешних отправителей событий по документу
        /// </summary>
        public List<int> SourceAgentId { get; set; }

        /// <summary>
        /// Массив ИД должностей получателей событий по документу
        /// </summary>
        public List<int> TargetPositionId { get; set; }
        /// <summary>
        /// Массив ИД агентов внутрених получателей событий по документу
        /// </summary>
        public List<int> TargetPositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД  департаментов получателей событий по документу
        /// </summary>
        public List<int> TargetDepartmentId { get; set; }
        /// <summary>
        /// Массив ИД агентов получателей событий по документу
        /// </summary>
        public List<int> TargetAgentId { get; set; }
    }
}
