using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessDefault_PositionCommand : BaseJournalAccessCommand
    {
        private ModifyAdminDefaultByPosition Model { get { return GetModel<ModifyAdminDefaultByPosition>(); } }

        public override object Execute()
        {
            var position = _dictDb.GetPosition(_context, Model.PositionId);

            if (position == null) return null;
            using (var transaction = Transactions.GetTransaction())
            {

                _adminDb.DeleteRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { PositionIDs = new List<int> { Model.PositionId } });

                //SetRegistrationJournalPositionByDepartment(new ModifyAdminRegistrationJournalPositionByDepartment
                //{
                //    DepartmentId = position.DepartmentId,
                //    IsChecked = true,
                //    PositionId = Model.PositionId,
                //    RegJournalAccessTypeId = EnumRegistrationJournalAccessTypes.View,
                //    IgnoreChildDepartments = true,
                //});

                SetRegistrationJournalPositionByDepartment(new SetJournalAccessByDepartment_Position
                {
                    DepartmentId = position.DepartmentId,
                    IsChecked = true,
                    PositionId = Model.PositionId,
                    RegJournalAccessTypeId = EnumRegistrationJournalAccessTypes.Registration,
                    IgnoreChildDepartments = true,
                });

                transaction.Complete();

            }

            return null;

        }

        protected void SetRegistrationJournalPositionByDepartment(SetJournalAccessByDepartment_Position model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetJournalAccessByDepartment_Position, _context, model);
        }
    }
}