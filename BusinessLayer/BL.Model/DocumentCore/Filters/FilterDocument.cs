﻿using BL.Model.SystemCore.Filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтра документов
    /// </summary>
    public class FilterDocument
    {


        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }

        /// <summary>
        /// Массив ИД документов полученного из полнотекстового поиска
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public List<int> FullTextSearchDocumentId { get; set; }
        /// <summary>
        /// Массив ИД шаблонов документов
        /// </summary>
        public List<int> TemplateDocumentId { get; set; }
        /// <summary>
        /// Массив ИД направлений документов
        /// </summary>
        public List<int> DocumentDirectionId { get; set; }
        /// <summary>
        /// Массив ИД типов документов
        /// </summary>
        public List<int> DocumentTypeId { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате создания
        /// </summary>
        public DateTime? CreateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате создания
        /// </summary>
        public DateTime? CreateToDate { get; set; }
        /// <summary>
        /// Массив ИД тематик документов
        /// </summary>
        public List<int> DocumentSubjectId { get; set; }
        /// <summary>
        /// Отрывок краткого содержания
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак регистрации документа
        /// </summary>
        public bool? IsRegistered { get; set; }
        /// <summary>
        /// Массив ИД журналов регистрации документов
        /// </summary>
        public List<int> RegistrationJournalId { get; set; }
        /// <summary>
        /// Отрывок полного регистрационного номера
        /// </summary>
        public string RegistrationNumber { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате регистрации документа
        /// </summary>
        public DateTime? RegistrationFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате регистрации документа
        /// </summary>
        public DateTime? RegistrationToDate { get; set; }
        /// <summary>
        /// Массив ИД ответственного по документу
        /// </summary>
        public List<int> ExecutorPositionId { get; set; }
        /// <summary>
        /// Массив ИД ответственного по документу
        /// </summary>
        public List<int> ExecutorPositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД ответственного по документу
        /// </summary>
        public List<int> ExecutorDepartmentId { get; set; }

        /// <summary>
        /// Массив ИД подписантов по документу
        /// </summary>
        public List<int> SubscriptionPositionId { get; set; }

        /// <summary>
        /// Массив ИД подписантов по документу
        /// </summary>
        public List<int> SubscriptionPositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД подписантов по документу
        /// </summary>
        public List<int> SubscriptionDepartmentId { get; set; }
        /// <summary>
        /// Массив ИД Task по документу
        /// </summary>
        public List<int> TaskId { get; set; }
        /// <summary>
        /// Отрывок краткого содержания по Task
        /// </summary>
        public string TaskDescription { get; set; }
        /// <summary>
        /// Массив ИД Tag по документу
        /// </summary>
        public List<int> TagId { get; set; }
        /// <summary>
        /// Отрывок краткого содержания по Tag
        /// </summary>
        public string TagDescription { get; set; }

        /// <summary>
        /// Массив ИД уровней доступа по документу
        /// </summary>
        public List<int> AccessLevelId { get; set; }

        /// <summary>
        /// В Моем управлении. true - выполнять поиск иначе ничего не делаеться
        /// </summary>
        public bool? IsInMyControl { get; set; }

        /// <summary>
        /// В избраном
        /// </summary>
        public bool? IsFavourite { get; set; }

        /// <summary>
        /// Массив ИД организаций
        /// </summary>
        public List<int> SenderAgentId { get; set; }
        /// <summary>
        /// Массив ИД контактов
        /// </summary>
        public List<int> SenderAgentPersonId { get; set; }
        /// <summary>
        /// Отрывок входящего номера документа
        /// </summary>
        public string SenderNumber { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате входящего документа
        /// </summary>
        public DateTime? SenderFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате входящего документа
        /// </summary>
        public DateTime? SenderToDate { get; set; }
        /// <summary>
        /// Отрывок кому адресован документ
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// Признак в работе
        /// </summary>
        public bool? IsInWork { get; set; } // should be true by default

        /// <summary>
        /// Содержит строку для поиска по полнотекстовому поиску. 
        /// </summary>
        public string FullTextSearch { get; set; }

        /// <summary>
        /// Фильтр по динамическим свойствам
        /// </summary>
        public List<FilterPropertyByRecord> FilterProperties { get; set; }

        /// <summary>
        /// Дата "с" для отбора по дате документа
        /// </summary>
        public DateTime? DocumentFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате документа
        /// </summary>
        public DateTime? DocumentToDate { get; set; }

        /// <summary>
        /// Отображать все документы
        /// </summary>
        public bool IsIgnoreRegistered { get; set; }
    }
}
