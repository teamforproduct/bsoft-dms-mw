using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.IncomingModel
{
    public class ModifyPropertyLink
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public EnumObjects Object { get; set; }
        public string Filers { get; set; }
        public bool IsMandatory { get; set; }
    }
}
