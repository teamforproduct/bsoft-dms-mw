using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.AdminCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.AdminCore
{
    public class ModifyPositionRoleCommand : BaseAdminCommand
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

            var filter = new FilterAdminPositionRole { NotContainsIDs = new List<int> { Model.Id }, PositionIDs = new List<int> { Model.PositionId }, RoleIDs = new List<int> { Model.RoleId } };

            if (_adminDb.ExistsPositionRole(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        {
            try
            {
                var model = CommonAdminUtilities.PositionRoleModifyToInternal(_context, Model);
                _adminDb.UpdatePositionRole(_context, model);
            }
            catch (AdminRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}