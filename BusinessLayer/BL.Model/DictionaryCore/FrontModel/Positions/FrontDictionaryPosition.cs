using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Штатное расписание". 
    /// </summary>
    public class FrontDictionaryPosition : ModifyPosition
    {

        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Индекс подразделения
        /// </summary>
        public string DepartmentCode { get; set; }
        
        /// <summary>
        /// Код подразделения
        /// </summary>
        public string DepartmentCodePath { get; set; }
        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Наименование компании
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Имя сотрудника на должности по штатному рассписанию
        /// </summary>
        public string AgentName { get; set; }


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
        /// Агент, который штатно занимает должность.
        /// </summary>
        [IgnoreDataMember]
        public int? MainExecutorAgentId { get; set; }

        /// <summary>
        /// Агент, который штатно занимает должность.
        /// </summary>
        [IgnoreDataMember]
        public string MainExecutorAgentName { get; set; }

        /// <summary>
        /// Наименование вышестоящей должности
        /// </summary>
        [IgnoreDataMember]
        public string ParentPositionName { get; set; }

        public int? MaxSubordinationTypeId { get; set; }

        /// <summary>
        /// Количество документов, к которым должность имеет доступ
        /// </summary>
        public int? DocCount { get; set; }

        /// <summary>
        /// Признак выбрана ли должность для работы
        /// </summary>
        public bool? IsChoosen { get; set; }

        public virtual IEnumerable<FrontDictionaryPosition> ChildPositions { get; set; }
        public virtual IEnumerable<FrontDictionaryDepartment> ChiefDepartments { get; set; }
        public virtual IEnumerable<FrontDictionaryStandartSendList> StandartSendLists { get; set; }

        public virtual IEnumerable<FrontDictionaryPositionExecutor> PositionExecutors { get; set; }

    }
}