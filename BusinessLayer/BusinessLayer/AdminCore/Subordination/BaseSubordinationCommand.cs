using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class BaseSubordinationCommand : BaseAdminCommand
    {

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            //_adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
          throw new NotImplementedException();
        }
        
    }
}