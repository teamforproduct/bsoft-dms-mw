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
        private int Model
        {
            get
            {
                if (!(_param is int)) throw new WrongParameterTypeError();
                return (int)_param;
            }
        }

        public override object Execute()
        {
            try
            {

                return "Done";
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }

        }
    }
}