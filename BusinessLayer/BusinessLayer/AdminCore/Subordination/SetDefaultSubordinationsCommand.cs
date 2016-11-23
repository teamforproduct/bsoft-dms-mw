using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetDefaultSubordinationsCommand : BaseSubordinationCommand
    {
        private ModifyAdminDefaultByPosition Model
        {
            get
            {
                if (!(_param is ModifyAdminDefaultByPosition)) throw new WrongParameterTypeError();
                return (ModifyAdminDefaultByPosition)_param;
            }
        }

        public override object Execute()
        {

            base.SetDefaultSubordinations(Model.PositionId, true, true);

            return null;

        }
    }
}