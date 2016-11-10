using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetDefaultRJournalPositionsCommand : BaseRJournalPositionCommand
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

            base.SetAll(Model.PositionId, true, true, true);

            return null;

        }
    }
}