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
    public class SetRolePermissionCommand : BaseRolePermissionCommand
    {

        protected SetRolePermission Model
        {
            get
            {
                if (!(_param is SetRolePermission)) throw new WrongParameterTypeError();
                return (SetRolePermission)_param;
            }
        }

        public override bool CanExecute() => IsModified(Model.RoleId);


        public override object Execute()
        {
            Set(Model.Module, Model.Feature, Model.AccessType, Model.RoleId, Model.IsChecked);

            return null;
        }
    }
}