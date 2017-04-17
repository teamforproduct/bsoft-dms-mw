using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие ролей и должности", представление записи.
    /// </summary>
    public class FrontAdminPositionRole 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public new int? PositionId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public new int? RoleId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Отмечено
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Заводская
        /// </summary>
        public bool IsDefault { get; set; }

    }
}