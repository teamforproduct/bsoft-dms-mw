using BL.Model.AdminCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Исполнители". 
    /// </summary>
    public class FrontDictionaryPositionExecutorExtended 
    {

        /// <summary>
        /// ID
        /// </summary>
        public int AssignmentId { get; set; }

        /// <summary>
        /// Признак активности.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Сотрудник-пользователь-агент (Id совпадают)
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Тип исполнения: 
        /// </summary>
        public EnumPositionExecutionTypes? PositionExecutorTypeId { get; set; }

        /// <summary>
        /// Уровень доступа к документам: лично, референт, ио
        /// При создании документов всегда указывается уровень доступа для ио и референтов
        /// </summary>
        public EnumAccessLevels? AccessLevelId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата начала исполнения должности
        /// </summary>
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate=value.ToUTC(); } }
        private DateTime?  _StartDate; 


        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        //[Required]
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate=value.ToUTC(); } }
        private DateTime?  _EndDate; 


        /// <summary>
        /// Агент
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionFullName { get; set; }

        /// <summary>
        /// Тип исполнителя
        /// </summary>
        public string PositionExecutorTypeName { get; set; }

        /// <summary>
        /// Краткое название (как приставка к названию должности) типа исполнителя
        /// </summary>
        public string PositionExecutorTypeSuffix { get; set; }

        /// <summary>
        /// Департамент
        /// </summary>
        public string DepartmentCodeName { get; set; }

        /// <summary>
        /// Уровень доступа: лично, референт, ио
        /// </summary>
        public string AccessLevelName { get; set; }

        /// <summary>
        ///  Роли должности
        /// </summary>
        public IEnumerable<FrontAdminPositionRole> PositionRoles { get; set; }

    }
}