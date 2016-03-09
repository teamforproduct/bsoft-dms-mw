using System;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.IncomingModel
{
    public class ModifyPropertyValue
    {
        public int PropertyLinkId { get; set; }
        public string Value { get; set; }
    }
}
