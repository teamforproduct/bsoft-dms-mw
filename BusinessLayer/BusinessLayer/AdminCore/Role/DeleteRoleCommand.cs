using BL.Logic.Common;
using BL.Model.Exception;

namespace BL.Logic.AdminCore
{
    public class DeleteRoleCommand : BaseAdminCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            string roleCode = _adminDb.GetRoleTypeCode(_context, Model);

            if (!string.IsNullOrEmpty(roleCode))
            {
                throw new DictionarySystemRecordCouldNotBeDeleted();
            }

            return true;
        }

        public override object Execute()
        {
            _adminDb.DeleteRole(_context, Model);
            return null;
        }
    }

}

