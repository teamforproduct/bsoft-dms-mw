using BL.Model.Common;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminSubordination
    /// </summary>
    public class FilterAdminSubordination: BaseFilter
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
        /// записи в которых должность как Source или Target
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// SubordinationType
        /// </summary>
        public List<EnumSubordinationTypes> SubordinationTypeIDs { get; set; }
    }
}