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
    public class ModifyRoleActionCommand : BaseAdminCommand
    {

        private ModifyAdminRoleAction Model
        {
            get
            {
                if (!(_param is ModifyAdminRoleAction)) throw new WrongParameterTypeError();
                return (ModifyAdminRoleAction)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminRoleAction {
                NotContainsIDs = new List<int> { Model.Id },
                ActionIDs = new List<int> { Model.ActionId },
                RoleIDs = new List<int> { Model.RoleId },
                RecordIDs = new List<int> { Model.RecordId ?? -1 }
            };

            if (_adminDb.ExistsRoleAction(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        {
            try
            {
                var dp = CommonAdminUtilities.RoleActionModifyToInternal(_context, Model);
                _adminDb.UpdateRoleAction(_context, dp);
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