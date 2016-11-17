using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace BL.Logic.AdminCore
{
    public class SetDefaultRJournalPositionsCommand : BaseRJournalPositionCommand
    {
        private ModifyAdminDefaultByPosition Model
        {
            get
            {
                if (!(_param is ModifyAdminDefaultByPosition)) throw new WrongParameterTypeError();
                return (ModifyAdminDefaultByPosition)_param;
            }
        }

        public override object Execute()
        {

            var position = _dictDb.GetPosition(_context, Model.PositionId);

            if (position == null) return null;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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

                SetRegistrationJournalPositionByDepartment(new ModifyAdminRegistrationJournalPositionByDepartment
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

        protected void SetRegistrationJournalPositionByDepartment(ModifyAdminRegistrationJournalPositionByDepartment model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetRegistrationJournalPositionByDepartment, _context, model);
        }
    }
}