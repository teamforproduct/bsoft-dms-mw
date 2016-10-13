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
        private ModifyAdminDefaultSubordination Model
        {
            get
            {
                if (!(_param is ModifyAdminDefaultSubordination)) throw new WrongParameterTypeError();
                return (ModifyAdminDefaultSubordination)_param;
            }
        }

        public override object Execute()
        {

            base.SetAllSubordinations(Model.PositionId, true, true, true);

            return null;

        }
    }
}