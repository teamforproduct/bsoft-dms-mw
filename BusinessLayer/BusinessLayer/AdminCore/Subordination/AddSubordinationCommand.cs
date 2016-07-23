using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddSubordinationCommand : BaseAdminCommand
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
            _admin.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminSubordination {
                SourcePositionIDs = new List<int> { Model.SourcePositionId },
                TargetPositionIDs = new List<int> { Model.TargetPositionId },
                SubordinationTypeIDs = new List<int> { Model.SubordinationTypeId }
            };

            if (_adminDb.ExistsSubordination(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        {
            try
            {
                var model = CommonAdminUtilities.SubordinationModifyToInternal(_context, Model);
                return _adminDb.AddSubordination(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}