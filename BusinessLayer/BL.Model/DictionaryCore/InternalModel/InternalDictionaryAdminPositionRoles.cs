using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryAdminPositionRoles : LastChangeInfo
    {

        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}