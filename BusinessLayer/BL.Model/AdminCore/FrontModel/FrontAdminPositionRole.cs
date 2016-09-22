using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие ролей и должности", представление записи.
    /// </summary>
    public class FrontAdminPositionRole : ModifyAdminPositionRole
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public new int? PositionId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public new int? RoleId { get; set; }

        ///// <summary>
        ///// Должность
        ///// </summary>
        //public string PositionName { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Отмечено
        /// </summary>
        public bool IsChecked { get; set; }

    }
}