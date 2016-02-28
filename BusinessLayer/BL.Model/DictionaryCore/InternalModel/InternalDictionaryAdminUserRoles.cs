using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryAdminUserRoles : LastChangeInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}