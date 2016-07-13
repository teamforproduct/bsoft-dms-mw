using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRole
    /// </summary>
    public class FilterAdminRole
    {

        /// <summary>
        /// Список ID
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// Сужение по наименованию элементов (по входжению)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по наименованию элементов (по равенству)
        /// </summary>
        public string NameExact { get; set; }

        /// <summary>
        /// Сужение по клиентам
        /// </summary>
        public List<int> ClientIDs { get; set; }
    }
}