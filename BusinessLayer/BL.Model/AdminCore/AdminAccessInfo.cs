using System.Collections.Generic;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Model.AdminCore
{
    public class AdminAccessInfo
    {
        public List<InternalDictionaryAdminRoleActions> ActionAccess { get; set; }
        public List<InternalDictionaryAdminUserRoles> UserRoles { get; set; }
        public List<InternalDictionaryAdminRoles> Roles { get; set; }
        public List<InternalDictionarySystemActions> Actions { get; set; }
    }
}