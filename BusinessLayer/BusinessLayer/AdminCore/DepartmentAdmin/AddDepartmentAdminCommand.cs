using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;

namespace BL.Logic.AdminCore
{
    public class AddDepartmentAdminCommand : BaseDepartmentAdminCommand
    {
        private AddAdminDepartmentAdmin Model { get { return GetModel<AddAdminDepartmentAdmin>(); } }

        public override object Execute()
        {
            var model = new InternalDepartmentAdmin(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);

            return _adminDb.AddDepartmentAdmin(_context, model);
        }
    }
}