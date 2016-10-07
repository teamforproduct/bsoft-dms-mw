using BL.Logic.Common;
using BL.Model.Exception;
using BL.Model.SystemCore.InternalModel;
using System;
using System.Collections.Generic;

namespace BL.Logic.SystemCore
{
    public class SetSettingCommand : BaseSettingCommand
    {
        public override object Execute()
        {
            try
            {
                var model = new InternalSystemSetting(Model);
                return _systemDb.AddSetting(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}