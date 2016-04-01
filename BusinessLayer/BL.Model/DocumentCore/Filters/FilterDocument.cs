using BL.Model.SystemCore.Filters;
using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтра документов
    /// </summary>
    public class FilterDocument
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FilterDocument()
        {
            IsInWork = true;
            DocumentId = new List<int>();
        }

        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }
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
        public List<int> SubscriptionrDepartmentId { get; set; }

        #region Event
        /// <summary>
        /// Отбор по событиям признаку прочтения Все/Только новые
        /// </summary>
        public bool? EventIsNew { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате событий документа
        /// </summary>
        public DateTime? EventFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате событий документа
        /// </summary>
        public DateTime? EventToDate { get; set; }
        /// <summary>
        /// Массив ИД типов событий
        /// </summary>
        public List<int> EventTypeId { get; set; }
        /// <summary>
        /// Массив ИД важности событий
        /// </summary>
        public List<int> EventImportanceEventTypeId { get; set; }
        /// <summary>
        /// Отрывок краткого содержания по событиям
        /// </summary>
        public string EventDescription { get; set; }

        /// <summary>
        /// Массив ИД отправителей событий по документу
        /// </summary>
        public List<int> EventSourcePositionId { get; set; }
        /// <summary>
        /// Массив ИД отправителей событий по документу
        /// </summary>
        public List<int> EventSourcePositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД отправителей событий по документу
        /// </summary>
        public List<int> EventSourceAgentId { get; set; }

        /// <summary>
        /// Массив ИД получателей событий по документу
        /// </summary>
        public List<int> EventTargetPositionId { get; set; }
        /// <summary>
        /// Массив ИД получателей событий по документу
        /// </summary>
        public List<int> EventTargetPositionExecutorAgentId { get; set; }
        /// <summary>
        /// Массив ИД получателей событий по документу
        /// </summary>
        public List<int> EventTargetAgentId { get; set; }
        #endregion Event
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

        #region Wait
        /// <summary>
        /// Дата "с" для отбора по дате срока исполнения контроля документа
        /// </summary>
        public DateTime? WaitDueDateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате срока исполнения контроля документа
        /// </summary>
        public DateTime? WaitDueDateToDate { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате возникновения  контроля документа
        /// </summary>
        public DateTime? WaitCreateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате возникновения  контроля документа
        /// </summary>
        public DateTime? WaitCreateToDate { get; set; }
        #endregion Wait

        #region File
        /// <summary>
        /// Отрывок названия по File
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Отрывок расширения по File
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// Число "с" для отбора по размеру файла документа
        /// </summary>
        public int? FileSizeFrom { get; set; }
        /// <summary>
        /// Число "по" для отбора по размеру файла документа
        /// </summary>
        public int? FileSizeTo { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате создания файла документа
        /// </summary>
        public DateTime? FileCreateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате создания файла документа
        /// </summary>
        public DateTime? FileCreateToDate { get; set; }
        /// <summary>
        /// Массив ИД пользователей по документу
        /// </summary>
        public List<int> FileAgentId { get; set; }
        #endregion File
        /// <summary>
        /// Массив ИД уровней доступа по документу
        /// </summary>
        public List<int> AccessLevelId { get; set; }

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

        public List<FilterPropertyByRecord> FilterProperties { get; set; }
    }
}
