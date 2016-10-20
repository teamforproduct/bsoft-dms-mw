using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DeletePositionRoleCommand : BaseAdminCommand
    {

        private ModifyAdminPositionRole Model
        {
            get
            {
                if (!(_param is ModifyAdminPositionRole)) throw new WrongParameterTypeError();
                return (ModifyAdminPositionRole)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var model = CommonAdminUtilities.PositionRoleModifyToInternal(_context, Model);
                _adminDb.DeletePositionRole(_context, model);
                // PSS При удалении роли у должности, удаляю роль у сотрудников, которые унасленовали ее от этой должности
                _adminDb.DeleteUserRoles(_context, new BL.Model.AdminCore.FilterModel.FilterAdminUserRole()
                {
                    PositionIDs = new List<int> { Model.PositionId },
                    RoleIDs = new List<int> { Model.RoleId}
                });
                return null;
            }
            catch (ArgumentNullException ex)
            {
                throw new AdminRecordWasNotFound(ex);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

