using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore.Filters;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetRolePermissionByModuleFeatureCommand : BaseRolePermissionCommand
    {

        protected SetRolePermissionByModuleFeature Model
        {
            get
            {
                if (!(_param is SetRolePermissionByModuleFeature)) throw new WrongParameterTypeError();
                return (SetRolePermissionByModuleFeature)_param;
            }
        }

        public override object Execute()
        {
            var list = _systemDb.GetInternalPermissions(_context, new FilterSystemPermissions
            {
                Module = Model.Module,
                Feature = Model.Feature
            });

            foreach (var item in list)
            {
                Set(Model.Module, Model.Feature, item.AccessTypeCode, Model.RoleId, Model.IsChecked);
            }

            return null;
        }
    }
}