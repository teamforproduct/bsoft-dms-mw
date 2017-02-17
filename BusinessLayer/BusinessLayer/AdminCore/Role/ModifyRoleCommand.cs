using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;


namespace BL.Logic.AdminCore
{
    public class ModifyRoleCommand : BaseRoleCommand
    {
        private ModifyAdminRole Model { get { return GetModel<ModifyAdminRole>(); } }

        public override object Execute()
        {
            var model = new InternalAdminRole(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            _adminDb.UpdateRole(_context, model);
            return null;
        }
    }
}