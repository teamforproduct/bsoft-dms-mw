using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.AdminCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.CrossCutting.Extensions;

namespace BL.Logic.AdminCore
{
    public class BaseUserRoleCommand : BaseAdminCommand
    {

        protected ModifyAdminUserRole Model
        {
            get
            {
                if (!(_param is ModifyAdminUserRole)) throw new WrongParameterTypeError();
                return (ModifyAdminUserRole)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            // Определяю нет ли в указанном интервале для сотрудника ролей, унаследованных от должности
            var filter = new FilterAdminUserRole
            {
                NotContainsIDs = new List<int> { Model.Id },
                RoleIDs = new List<int> { Model.RoleId },
                //UserIDs = new List<int> { Model.UserId },
                PositionExecutorIDs = new List<int> { Model.PositionExecutorId },
            };

            if (_adminDb.ExistsUserRole(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}