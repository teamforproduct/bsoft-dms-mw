using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore.Filters;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetRolePermissionByModuleAccessTypeCommand : BaseRolePermissionCommand
    {

        protected SetRolePermissionByModuleAccessType Model
        {
            get
            {
                if (!(_param is SetRolePermissionByModuleAccessType)) throw new WrongParameterTypeError();
                return (SetRolePermissionByModuleAccessType)_param;
            }
        }

        public override bool CanExecute() => IsModified(Model.RoleId);

        public override object Execute()
        {
            var list = _systemDb.GetInternalPermissions(_context, new FilterSystemPermissions
            {
                Module = Model.Module,
                AccessType = Model.AccessType
            });

            foreach (var item in list)
            {
                Set(Model.Module, item.FeatureCode, Model.AccessType, Model.RoleId, Model.IsChecked);
            }

            return null;
        }
    }
}