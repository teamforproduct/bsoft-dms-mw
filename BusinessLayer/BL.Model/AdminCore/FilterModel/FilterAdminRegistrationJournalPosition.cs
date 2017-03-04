using BL.Model.Common;
using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRegistrationJournalPosition
    /// </summary>
    public class FilterAdminRegistrationJournalPosition: BaseFilter
    {

        /// <summary>
        /// PositionIDs
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// RegistrationJournalIDs
        /// </summary>
        public List<int> RegistrationJournalIDs { get; set; }

        /// <summary>
        /// RegistrationJournalAccessTypeIDs
        /// </summary>
        public List<EnumRegistrationJournalAccessTypes> RegistrationJournalAccessTypeIDs { get; set; }
    }
}