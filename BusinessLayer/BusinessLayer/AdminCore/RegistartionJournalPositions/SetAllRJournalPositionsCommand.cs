using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetAllRJournalPositionsCommand : BaseRJournalPositionCommand
    {
        private ModifyAdminRegistrationJournalPositions Model
        {
            get
            {
                if (!(_param is ModifyAdminRegistrationJournalPositions)) throw new WrongParameterTypeError();
                return (ModifyAdminRegistrationJournalPositions)_param;
            }
        }

        public override object Execute()
        {
            SetAll(Model.PositionId, Model.IsChecked, 
                Model.RegJournalAccessTypeId == BL.Model.Enums.EnumRegistrationJournalAccessTypes.View,
                Model.RegJournalAccessTypeId == BL.Model.Enums.EnumRegistrationJournalAccessTypes.Registration);
            return null;
        }

    }
}