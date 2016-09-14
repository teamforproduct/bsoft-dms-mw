using System.Collections.Generic;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.AdminCore.InternalModel;

namespace BL.Model.AdminCore
{
    public class AdminAccessInfo
    {
        public List<InternalAdminRoleAction> ActionAccess { get; set; }
        public List<InternalAdminUserRole> UserRoles { get; set; }
        public List<InternalAdminRole> Roles { get; set; }
        public List<InternalAdminPositionRole> PositionRoles { get; set; }
        public List<InternalDictionarySystemActions> Actions { get; set; }
    }
}