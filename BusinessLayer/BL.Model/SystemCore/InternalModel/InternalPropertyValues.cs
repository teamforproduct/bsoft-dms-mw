using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalPropertyValues
    {
        public int RecordId { get; set; }
        public EnumObjects Object { get; set; }
        public IEnumerable<InternalPropertyValue> PropertyValues { get; set; }
    }
}