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
    public class FrontUserAssignments
    {

        /// <summary>
        /// Дата начала исполнения ролей должности
        /// </summary>
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate=value.ToUTC(); } }
        private DateTime? _StartDate; 


        /// <summary>
        /// Дата окончания исполнения ролей должности
        /// </summary>
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate=value.ToUTC(); } }
        private DateTime? _EndDate; 


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
        /// ИД Тип исполнения
        /// </summary>
        public int RolePositionExecutorTypeId { get; set; }
        /// <summary>
        /// Тип исполнения
        /// </summary>
        public string RolePositionExecutorTypeName { get; set; }
        /// <summary>
        /// Признак, выбиралась ли позиция при последнем входе
        /// </summary>
        public bool IsLastChosen { get; set; }
        /// <summary>
        /// Количество непрочитанных событий
        /// </summary>
        public int? NewEventsCount { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public string DepartmentName { get; set; }

    }
}