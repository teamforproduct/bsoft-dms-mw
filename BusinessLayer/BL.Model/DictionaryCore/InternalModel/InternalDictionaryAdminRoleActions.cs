using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryAdminRoleActions : LastChangeInfo
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ActionId { get; set; }
        public int? RecordId { get; set; }
    }
}