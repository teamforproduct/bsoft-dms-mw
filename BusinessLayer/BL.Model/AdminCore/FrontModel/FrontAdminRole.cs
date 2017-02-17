using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Список ролей", представление записи.
    /// </summary>
    public class FrontAdminRole 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Классификатор роли
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Описание роли
        /// </summary>
        public string Description { get; set; }
    }
}