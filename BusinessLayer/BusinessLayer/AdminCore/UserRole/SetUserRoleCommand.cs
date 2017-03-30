using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.AdminCore
{
    public class SetUserRoleCommand : BaseAdminCommand
    {

        private SetUserRole Model { get { return GetModel<SetUserRole>(); } }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var assignment = _dictDb.GetPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { Model.PositionExecutorId } }).FirstOrDefault();

            if (assignment == null) throw new DictionaryRecordWasNotFound();

            var existsPositionRoles = _adminDb.GetPositionRoles(_context, new FilterAdminPositionRole
            {
                RoleIDs = new List<int> { Model.Id },
                PositionIDs = new List<int> { assignment.PositionId }
            }).Any();

            if (!existsPositionRoles) throw new AdminRecordCouldNotBeAdded();

            return true;
        }


        public override object Execute()
        {
            var model = new InternalAdminUserRole(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            var filter = new FilterAdminUserRole
            {
                PositionExecutorIDs = new List<int> { Model.PositionExecutorId },
                RoleIDs = new List<int> { Model.Id }
            };

            var exists = _adminDb.ExistsUserRole(_context, filter);

            if (!exists && Model.IsChecked)
            {
                _adminDb.AddUserRoles(_context, new List<InternalAdminUserRole> { model });
            }
            else if (exists && !Model.IsChecked)
            {
                _adminDb.DeleteUserRoles(_context, filter);

            }

            return null;

        }

    }
}