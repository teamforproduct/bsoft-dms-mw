using BL.Model.Extensions;
using BL.Model.SystemCore.Filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр ожиданий документов
    /// </summary>
    public class FilterDocumentWait
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
        /// ИД. инициирующего события
        /// </summary>
        public List<int> OnEventId { get; set; }
        /// <summary>
        /// ИД. закрывающего события. 
        /// </summary>
        public List<int> OffEventId { get; set; }

        /// <summary>
        ///Если истина, то фильтруются ожидания, где OffEvent = null
        ///Если лож, то фильтруются ожидания, где OffEvent != null
        ///Если null(не задан), то фильтр не применяется
        /// </summary>
        public bool? IsOpened { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате срока исполнения контроля документа
        /// </summary>
        public DateTime? DueDateFromDate { get { return _DueDateFromDate; } set { _DueDateFromDate = value.ToUTC(); } }
        private DateTime? _DueDateFromDate;
        /// <summary>
        /// Дата "по" для отбора по дате срока исполнения контроля документа
        /// </summary>
        public DateTime? DueDateToDate { get { return _DueDateToDate; } set { _DueDateToDate = value.ToUTC(); } }
        private DateTime? _DueDateToDate;
        /// <summary>
        /// Дата "с" для отбора по дате возникновения  контроля документа
        /// </summary>
        public DateTime? CreateFromDate { get { return _CreateFromDate; } set { _CreateFromDate = value.ToUTC(); } }
        private DateTime? _CreateFromDate;
        /// <summary>
        /// Дата "по" для отбора по дате возникновения  контроля документа
        /// </summary>
        public DateTime? CreateToDate { get { return _CreateToDate; } set { _CreateToDate = value.ToUTC(); } }
        private DateTime? _CreateToDate;
        /// <summary>
        /// Массив ИД отправителей контроля по документу
        /// </summary>
        public List<int> ControlToMePositionId { get; set; }
        /// <summary>
        /// Массив ИД отправителей контроля по документу
        /// </summary>
        public List<int> ControlToMePositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД отправителей контроля по документу
        /// </summary>
        public List<int> ControlToMeDepartmentId { get; set; }
        /// <summary>
        /// Массив ИД получателей контроля по документу
        /// </summary>
        public List<int> ControlFromMePositionId { get; set; }
        /// <summary>
        /// Массив ИД получателей контроля по документу
        /// </summary>
        public List<int> ControlFromMePositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД получателей контроля по документу
        /// </summary>
        public List<int> ControlFromMeDepartmentId { get; set; }
        /// <summary>
        /// Массив ИД получателей контроля по документу
        /// </summary>
        public List<int> ControlFromMeAgentId { get; set; }

        /// <summary>
        /// Самоконтроль. true - выполнять поиск иначе ничего не делаеться
        /// </summary>
        public bool? IsSelfControl { get; set; }
        /// <summary>
        /// Поступившие на визирование. true - выполнять поиск иначе ничего не делаеться
        /// </summary>
        public bool? IsVisaingToMe { get; set; }

        /// <summary>
        /// Отправленные на визирование. true - выполнять поиск иначе ничего не делаеться
        /// </summary>
        public bool? IsVisaingFromMe { get; set; }
        /// <summary>
        /// Отчеты о выполнении. true - выполнять поиск иначе ничего не делаеться
        /// </summary>
        public bool? IsMarkExecution { get; set; }
        /// <summary>
        ///Если истина - просроченные, ложь - непросроченные, если null(не задан), то фильтр не применяется
        /// </summary>
        public bool? IsOverDue { get; set; }
        /// <summary>
        /// Мои контроли - от меня или мне. true - выполнять поиск иначе ничего не делаеться
        /// </summary>
        public bool? IsMyControl { get; set; }
    }
}
