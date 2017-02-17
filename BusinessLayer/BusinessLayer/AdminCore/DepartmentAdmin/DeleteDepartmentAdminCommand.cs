namespace BL.Logic.AdminCore
{
    public class DeleteDepartmentAdminCommand : BaseDepartmentAdminCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override object Execute()
        {
            _adminDb.DeleteDepartmentAdmin(_context, Model);
            return null;
        }
    }
}