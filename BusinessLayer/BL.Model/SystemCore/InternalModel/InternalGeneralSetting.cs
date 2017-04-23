using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalGeneralSetting
    {

        public InternalGeneralSetting() { }

        public InternalGeneralSetting(ModifyGeneralSetting model)
        {
            Key = model.Key;
            Value = model.Value;
            //ValueType = model.ValueType;
        }
        public string Key { get; set; }
        public string Value { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public EnumValueTypes ValueType { get; set; }

        public int Order { get; set; }

    }
}
