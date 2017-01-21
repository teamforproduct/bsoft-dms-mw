using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Список ролей", представление записи.
    /// </summary>
    public class FrontModule
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список фич
        /// </summary>
        public IEnumerable<FrontFeature> Features { get; set; }

    }
}