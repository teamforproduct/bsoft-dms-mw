using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRoleAction
    /// </summary>
    public class FilterAdminRoleActionDIP: AdminBaseFilterParameters
    {
        /// <summary>
        /// Описание действия
        /// </summary>
        public string ActionDescription { get; set; }

        /// <summary>
        /// Действия
        /// </summary>
        public List<int> ActionIDs { get; set; }

        /// <summary>
        /// Отмечнено
        /// </summary>
        public bool? IsChecked { get; set; }


    }
}