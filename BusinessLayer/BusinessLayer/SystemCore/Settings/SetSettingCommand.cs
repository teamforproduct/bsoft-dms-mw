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
                var res = new List<int>();
                foreach (var model in Model)
                {
                    var modelInt = new InternalSystemSetting(model);
                    res.Add(_systemDb.MergeSetting(_context, modelInt));
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}