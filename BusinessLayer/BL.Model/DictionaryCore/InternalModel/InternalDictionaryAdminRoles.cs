using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryAdminRoles : LastChangeInfo
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}