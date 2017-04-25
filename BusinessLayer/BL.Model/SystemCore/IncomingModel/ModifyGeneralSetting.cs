using System.ComponentModel.DataAnnotations;

namespace BL.Model.SystemCore.IncomingModel
{
    public class ModifyGeneralSetting
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
        //[Required]
        //public EnumValueTypes ValueType { get; set; }

    }
}
