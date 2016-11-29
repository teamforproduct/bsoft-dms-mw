﻿using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.AdminCore
{
    public class DeleteUserRolesByPositionExecutorCommand : BaseAdminCommand
    {

        private int Model
        {
            get
            {
                if (!(_param is int)) throw new WrongParameterTypeError();
                return (int)_param;
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
                _adminDb.DeleteUserRoles(_context, new BL.Model.AdminCore.FilterModel.FilterAdminUserRole { PositionExecutorIDs = new System.Collections.Generic.List<int> { Model } });
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
