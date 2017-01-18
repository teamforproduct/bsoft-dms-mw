using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore.Filters;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetRolePermissionByFeatureCommand : BaseRolePermissionCommand
    {

        protected SetRolePermissionByFeature Model
        {
            get
            {
                if (!(_param is SetRolePermissionByFeature)) throw new WrongParameterTypeError();
                return (SetRolePermissionByFeature)_param;
            }
        }

        public override object Execute()
        {
            var list = _systemDb.GetPermissions(_context, new FilterSystemPermissions { FeatureIDs = new List<int> { Model.FeatureId } });

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