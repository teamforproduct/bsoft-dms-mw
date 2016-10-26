using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DeleteRoleActionCommand : BaseAdminCommand
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
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var filter = new FilterAdminRoleAction()
                {
                    RoleIDs = new List<int> { Model.RoleId },
                    ActionIDs = new List<int> { Model.ActionId },
                };
                _adminDb.DeleteRoleActions(_context, filter);
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

