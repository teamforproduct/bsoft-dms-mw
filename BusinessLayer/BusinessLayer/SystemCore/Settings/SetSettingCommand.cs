using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
                foreach (var model in Model)
                {
                    var modelInt = new InternalSystemSetting(model);
                    var sett = DmsResolver.Current.Get<ISettings>();
                    sett.SaveSetting(_context, modelInt);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}