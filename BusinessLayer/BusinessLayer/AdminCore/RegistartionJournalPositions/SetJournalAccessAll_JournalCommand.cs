using BL.Model.AdminCore.IncomingModel;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessAll_JournalCommand : BaseJournalAccessCommand
    {
        private SetJournalAccessAll_Journal Model { get { return GetModel<SetJournalAccessAll_Journal>(); } }

        public override object Execute()
        {
            SetAll_Journal(Model.JournalId, Model.IsChecked, 
                Model.RegJournalAccessTypeId == BL.Model.Enums.EnumRegistrationJournalAccessTypes.View,
                Model.RegJournalAccessTypeId == BL.Model.Enums.EnumRegistrationJournalAccessTypes.Registration);
            return null;
        }

    }
}