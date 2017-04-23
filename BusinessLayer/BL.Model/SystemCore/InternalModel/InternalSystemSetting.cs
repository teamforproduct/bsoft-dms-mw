using BL.Model.SystemCore.IncomingModel;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalSystemSetting : InternalGeneralSetting
    {

        public InternalSystemSetting() { }

        public InternalSystemSetting(ModifySystemSetting model) : base(new ModifyGeneralSetting { Key = model.Key, Value = model.Value })
        {
            AgentId = model.AgentId;
        }
        public int? AgentId { get; set; }

        public int SettingTypeId { get; set; }

        public int AccessType { get; set; }

    }
}
