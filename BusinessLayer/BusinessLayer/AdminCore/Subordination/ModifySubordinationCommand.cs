using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.AdminCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.AdminCore
{
    public class ModifySubordinationCommand : BaseAdminCommand
    {

        private ModifyAdminSubordination Model
        {
            get
            {
                if (!(_param is ModifyAdminSubordination)) throw new WrongParameterTypeError();
                return (ModifyAdminSubordination)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminSubordination
            {
                NotContainsIDs = new List<int> { Model.Id },
                SourcePositionIDs = new List<int> { Model.SourcePositionId },
                TargetPositionIDs = new List<int> { Model.TargetPositionId },
                SubordinationTypeIDs = new List<int> { (int)Model.SubordinationTypeId }
            };

            if (_adminDb.ExistsSubordination(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        {
                try
                {
                var dp = CommonAdminUtilities.SubordinationModifyToInternal(_context, Model);
                _adminDb.UpdateSubordination(_context, dp);
            }
                catch (AdminRecordWasNotFound)
                {
                throw;
            }
                catch (Exception ex)
                {
                throw new DatabaseError(ex);
            }
                return null;
        }
    }
}