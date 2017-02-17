using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;

namespace BL.Logic.AdminCore
{
    public class AddRoleCommand : BaseRoleCommand
    {
        private AddAdminRole Model { get { return GetModel<AddAdminRole>(); } }

        public override object Execute()
        {
            var model = new InternalAdminRole(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            return _adminDb.AddRole(_context, model);
        }
    }
}