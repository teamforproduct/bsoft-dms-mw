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
        public int? UserId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// Назначение
        /// </summary>
        public int? PositionExecutorId { get; set; }

        ///// <summary>
        ///// Дата назначения роли
        ///// </summary>
        //public DateTime? StartDate { get; set; }

        ///// <summary>
        ///// Дата снятия роли
        ///// </summary>
        //public DateTime? EndDate { get; set; }


        ///// <summary>
        ///// Пользователь
        ///// </summary>
        //public string UserName { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public string RoleName { get; set; }

        ///// <summary>
        ///// Отмечено
        ///// </summary>
        //public bool? IsChecked { get; set; }

        /// <summary>
        /// Должность, от которой унаследована роль
        /// </summary>
        public int? RolePositionId { get; set; }


    }
}