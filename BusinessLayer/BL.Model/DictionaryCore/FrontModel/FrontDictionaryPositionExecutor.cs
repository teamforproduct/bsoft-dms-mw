using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using System;
using BL.Model.Enums;
using System.Runtime.Serialization;
using BL.Model.AdminCore.FrontModel;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Исполнители". 
    /// </summary>
    public class FrontDictionaryPositionExecutor 
    {

        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

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
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        //[Required]
        public DateTime? EndDate { get; set; }

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
        /// Уровень доступа: лично, референт, ио
        /// </summary>
        public string AccessLevelName { get; set; }

        /// <summary>
        ///  Роли должности
        /// </summary>
        public IEnumerable<FrontAdminPositionRole> PositionRoles { get; set; }

    }
}