using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminSubordination
    /// </summary>
    public class FilterAdminSubordination: AdminBaseFilterParameters
    {
      
        /// <summary>
        /// SourcePosition
        /// </summary>
        public List<int> SourcePositionIDs { get; set; }

        /// <summary>
        /// TargetPosition
        /// </summary>
        public List<int> TargetPositionIDs { get; set; }

        /// <summary>
        /// SubordinationType
        /// </summary>
        public List<int> SubordinationTypeIDs { get; set; }
    }
}