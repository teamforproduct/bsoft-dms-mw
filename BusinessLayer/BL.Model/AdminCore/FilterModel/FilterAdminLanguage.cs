using System.Collections.Generic;

namespace BL.Model.AdminCore.FilterModel
{
    public class FilterAdminLanguage
    {
        public List<int> IDs { get; set; }
        public string Code { get; set; }
        public bool? IsDefault { get; set; }
    }
}
