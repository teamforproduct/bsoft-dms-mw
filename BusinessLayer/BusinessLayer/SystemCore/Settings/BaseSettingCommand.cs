using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.Model.SystemCore.IncomingModel;

namespace BL.Logic.SystemCore
{
    public class BaseSettingCommand : BaseSystemCommand
    {

        protected ModifySystemSetting Model
        {
            get
            {
                if (!(_param is ModifySystemSetting)) throw new WrongParameterTypeError();
                return (ModifySystemSetting)_param;
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