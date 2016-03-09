using BL.Model.Common;
using BL.Model.Enums;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalPropertyLink : LastChangeInfo
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public EnumObjects Object { get; set; }
        public string Filers { get; set; }
        public bool IsMandatory { get; set; }
        
        public InternalProperty Property { get; set; }
    }
}