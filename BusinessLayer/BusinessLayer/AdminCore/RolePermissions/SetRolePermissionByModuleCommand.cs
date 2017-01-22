using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore.Filters;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetRolePermissionByModuleCommand : BaseRolePermissionCommand
    {

        protected SetRolePermissionByModule Model
        {
            get
            {
                if (!(_param is SetRolePermissionByModule)) throw new WrongParameterTypeError();
                return (SetRolePermissionByModule)_param;
            }
        }

        public override object Execute()
        {
            var list = _systemDb.GetInternalPermissions(_context, new FilterSystemPermissions
            {
                Module = Model.Module
            });

            foreach (var item in list)
            {
                Set(Model.Module, item.FeatureCode, item.AccessTypeCode, Model.RoleId, Model.IsChecked);
            }

            return null;
        }
    }
}