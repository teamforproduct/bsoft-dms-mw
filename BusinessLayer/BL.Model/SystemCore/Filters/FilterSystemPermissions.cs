using BL.Model.AdminCore.FilterModel;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Фильтры FilterAdminUserPermissions
    /// </summary>
    public class FilterSystemPermissions : AdminBaseFilterParameters
    {

        /// <summary>
        /// ModuleIDs
        /// </summary>
        public List<int> ModuleIDs { get; set; }

        /// <summary>
        /// FeatureIDs
        /// </summary>
        public List<int> FeatureIDs { get; set; }


    }

}