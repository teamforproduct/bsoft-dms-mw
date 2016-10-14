using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие ролей и пользователя", описывает доступные пользователю роли, представление записи.
    /// </summary>
    public class FrontAdminUserRole 
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Сотрудник
        /// </summary>
        public new int? UserId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public new int? RoleId { get; set; }

        /// <summary>
        /// Дата назначения роли
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата снятия роли
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Тип объекта. Например: Конкретному сотруднику на конр должности разрешены действия (роль) над конкретным объектом.
        /// </summary>
        public EnumObjects? ObjectId { get; set; }

        /// <summary>
        /// Id сущности. Например отдел...
        /// </summary>
        public int? EntityId { get; set; }

        

        ///// <summary>
        ///// Пользователь
        ///// </summary>
        //public string UserName { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Отмечено
        /// </summary>
        public bool? IsChecked { get; set; }


        public int? RolePositionId { get; set; }
        public string RolePositionName { get; set; }
        public string RolePositionExecutorAgentName { get; set; }
        public int? NewEventsCount { get; set; }

    }
}