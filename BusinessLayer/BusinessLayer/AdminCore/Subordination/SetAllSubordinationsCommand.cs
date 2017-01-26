using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetAllSubordinationsCommand : BaseSubordinationCommand
    {
        private SetAdminSubordinations Model
        {
            get
            {
                if (!(_param is SetAdminSubordinations)) throw new WrongParameterTypeError();
                return (SetAdminSubordinations)_param;
            }
        }

        public override object Execute()
        {
            SetAllSubordinations(Model.PositionId, Model.IsChecked,
                Model.SubordinationTypeId == BL.Model.Enums.EnumSubordinationTypes.Execution,
                Model.SubordinationTypeId == BL.Model.Enums.EnumSubordinationTypes.Informing);
            return null;
        }

    }
}