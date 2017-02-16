using BL.Model.AdminCore.IncomingModel;
using BL.Model.Exception;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessAll_PositionCommand : BaseJournalAccessCommand
    {
        private SetJournalAccessAll_Position Model { get { return GetModel<SetJournalAccessAll_Position>(); } }

        public override object Execute()
        {
            SetAll_Position(Model.PositionId, Model.IsChecked, 
                Model.RegJournalAccessTypeId == BL.Model.Enums.EnumRegistrationJournalAccessTypes.View,
                Model.RegJournalAccessTypeId == BL.Model.Enums.EnumRegistrationJournalAccessTypes.Registration);
            return null;
        }

    }
}