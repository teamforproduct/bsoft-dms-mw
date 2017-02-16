using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessByDepartment_JournalCommand : BaseJournalAccessCommand
    {
        private SetJournalAccessByDepartment_Journal Model { get { return GetModel<SetJournalAccessByDepartment_Journal>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                // Устанавливаю рассылку для должностей заданного отдела
                SetByDepartment_Journal(Model.DepartmentId, Model.JournalId, Model.IsChecked, Model.RegJournalAccessTypeId);

                if (!Model.IgnoreChildDepartments)
                {
                    // Устанавливаю рассылку для должностей дочерних отделов
                    SetForChildDepartments_Journal(Model.DepartmentId, Model.JournalId, Model.IsChecked, Model.RegJournalAccessTypeId);
                }

                transaction.Complete();
            }

            return null;
        }

        
    }
}