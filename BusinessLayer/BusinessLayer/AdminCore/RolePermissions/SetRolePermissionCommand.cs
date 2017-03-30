using BL.Model.AdminCore.IncomingModel;
using BL.Model.Exception;


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