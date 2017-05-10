using System;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FrontModel;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentAccess
    {
        /// <summary>
        /// ИД доступа
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД должности
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// Наименование должности
        /// </summary>
        public string PositionName { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        public int? AccessLevelId { get; set; }
        /// <summary>
        /// Название уровня доступа
        /// </summary>
        public string AccessLevelName { get; set; }
        /// <summary>
        /// Признак в работе
        /// </summary>
        public bool? IsInWork { get; set; }
        /// <summary>
        /// Признак фаворита
        /// </summary>
        public bool? IsFavourite { get; set; }
        /// <summary>
        /// Количество новых событий
        /// </summary>
        public int? CountNewEvents { get; set; }
        /// <summary>
        /// Количество открытых контролей
        /// </summary>
        public int? CountWaits { get; set; }
        /// <summary>
        /// Количество просроченных контролей
        /// </summary>
        public int? OverDueCountWaits { get; set; }
        /// <summary>
        /// Дата ближайшего контроля
        /// </summary>
        public DateTime? MinDueDate { get; set; }

        /// <summary>
        /// Признак выбрана ли должность для работы
        /// </summary>
        public bool? IsChoosen { get; set; }
        /// <summary>
        /// ИД подразделения, в которое включена должность
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Текущий на должности (Выполняет действия в оперативной деятельности)
        /// </summary>
        //[IgnoreDataMember]
        public int? ExecutorAgentId { get; set; }
        /// <summary>
        /// Выполняет действия в оперативной деятельности
        /// </summary>
        //[IgnoreDataMember]
        public string ExecutorAgentName { get; set; }
        /// <summary>
        /// Количество документов, к которым должность имеет доступ
        /// </summary>
        public int? DocCount { get; set; }
    }
}
