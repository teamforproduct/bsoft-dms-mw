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
            var list = _systemDb.GetPermissions(_context, new FilterSystemPermissions { FeatureIDs = new List<int> { Model.ModuleId } });

            foreach (var item in list)
            {
                var row = new InternalAdminRolePermission()
                {
                    PermissionId = item.Id,
                    RoleId = Model.RoleId,
                };

                Set(row, Model.IsChecked);
            }

            return null;
        }
    }
}