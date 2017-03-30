using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.SystemCore.IncomingModel;
using System;
using System.Collections.Generic;

namespace BL.Logic.SystemCore
{
    public class BaseSettingCommand : BaseSystemCommand
    {

        protected List<ModifySystemSetting> Model
        {
            get
            {
                if (!(_param is List<ModifySystemSetting>)) throw new WrongParameterTypeError();
                return (List<ModifySystemSetting>)_param;
            }
        }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}