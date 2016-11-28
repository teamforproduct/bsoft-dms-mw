using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalSystemSetting
    {

        public InternalSystemSetting() { }

        public InternalSystemSetting(ModifySystemSetting model)
        {
            Key = model.Key;
            Value = model.Value;
            ValueType = model.ValueType;
            AgentId = model.AgentId;
        }
        public string Key { get; set; }
        public string Value { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public EnumValueTypes ValueType { get; set; }
        public int? AgentId { get; set; }

        public int SettingTypeId { get; set; }

        public int Order { get; set; }

        public int AccessType { get; set; }

    }
}
