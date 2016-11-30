using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие ролей и пользователя", описывает доступные пользователю роли, представление записи.
    /// </summary>
    public class FrontAvailablePositions
    {

        /// <summary>
        /// Дата начала исполнения ролей должности
        /// </summary>
        private DateTime? _StartDate; 
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate=value.ToUTC(); } }


        /// <summary>
        /// Дата окончания исполнения ролей должности
        /// </summary>
        private DateTime? _EndDate; 
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate=value.ToUTC(); } }


        /// <summary>
        /// Роль
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Должность, от которой унаследована роль
        /// </summary>
        public int? RolePositionId { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        public string RolePositionName { get; set; }

        /// <summary>
        /// Имя сотрудника
        /// </summary>
        public string RolePositionExecutorAgentName { get; set; }

        /// <summary>
        /// Тип исполнения
        /// </summary>
        public string RolePositionExecutorTypeName { get; set; }

        public int? NewEventsCount { get; set; }

    }
}