using BL.Model.AdminCore.FilterModel;
using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.SystemCore.Filters
{
    /// <summary>
    /// Фильтры FilterAdminUserPermissions
    /// </summary>
    public class FilterSystemPermissions : BaseFilter
    {

        /// <summary>
        /// Module
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Feature
        /// </summary>
        public string Feature { get; set; }


        /// <summary>
        /// Feature
        /// </summary>
        public string AccessType { get; set; }

        /// <summary>
        /// ModuleIDs
        /// </summary>
        public List<int> ModuleIDs { get; set; }

        /// <summary>
        /// FeatureIDs
        /// </summary>
        public List<int> FeatureIDs { get; set; }


        /// <summary>
        /// FeatureIDs
        /// </summary>
        public List<int> AccessTypeIDs { get; set; }

    }

}