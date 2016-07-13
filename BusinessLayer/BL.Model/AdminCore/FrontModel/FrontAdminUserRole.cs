using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие ролей и пользователя", описывает доступные пользователю роли, представление записи.
    /// </summary>
    public class FrontAdminUserRole : ModifyAdminUserRole
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public int UserName { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int RoleName { get; set; }


        public int RolePositionId { get; set; }
        public string RolePositionName { get; set; }
        public string RolePositionExecutorAgentName { get; set; }
        public int NewEventsCount { get; set; }

    }
}