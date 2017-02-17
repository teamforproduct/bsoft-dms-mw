using BL.Logic.Common;
using System;

namespace BL.Logic.AdminCore
{
    public class BaseDepartmentAdminCommand : BaseAdminCommand
    {

        

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            //_adminService.VerifyAccess(_context, CommandType, false);

            //var filter = new FilterAdminPositionRole
            //{
            //    NotContainsIDs = new List<int> { Model.Id },
            //    PositionIDs = new List<int> { Model.PositionId },
            //    RoleIDs = new List<int> { Model.RoleId }
            //};

            //if (_adminDb.ExistsPositionRole(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }


        public override object Execute()
        { throw new NotImplementedException(); }

    }
}