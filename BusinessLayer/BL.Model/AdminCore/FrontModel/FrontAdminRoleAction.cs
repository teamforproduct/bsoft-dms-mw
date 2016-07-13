using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие действий и роли", представление записи.
    /// </summary>
    public class FrontAdminRoleAction : ModifyAdminRoleAction
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public int ActionName { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int RoleName { get; set; }

    }
}