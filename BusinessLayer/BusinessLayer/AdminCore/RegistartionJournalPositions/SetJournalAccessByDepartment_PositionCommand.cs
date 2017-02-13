using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessByDepartment_PositionCommand : BaseJournalAccessCommand
    {
        private SetJournalAccessByDepartment_Position Model { get { return GetModel<SetJournalAccessByDepartment_Position>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                // Устанавливаю рассылку для должностей заданного отдела
                SetByDepartment_Position(Model.DepartmentId, Model.PositionId, Model.IsChecked, Model.RegJournalAccessTypeId);

                if (!Model.IgnoreChildDepartments)
                {
                    // Устанавливаю рассылку для должностей дочерних отделов
                    SetForChildDepartments_Position(Model.DepartmentId, Model.PositionId, Model.IsChecked, Model.RegJournalAccessTypeId);
                }

                transaction.Complete();
            }

            return null;
        }

    }
}