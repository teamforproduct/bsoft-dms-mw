using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Список ролей", представление записи.
    /// </summary>
    public class FrontAdminRole : ModifyAdminRole
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; }

    }
}