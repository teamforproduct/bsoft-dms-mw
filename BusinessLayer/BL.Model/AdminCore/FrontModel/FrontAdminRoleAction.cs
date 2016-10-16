using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие действий и роли", представление записи.
    /// </summary>
    public class FrontAdminRoleAction 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public string ActionDescription{ get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Отмечено
        /// </summary>
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public int? ActionId { get; set; }

        /// <summary>
        /// RecordId
        /// </summary>
        public int? RecordId { get; set; }
    }
}