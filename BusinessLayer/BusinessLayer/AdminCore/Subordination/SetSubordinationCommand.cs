using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetSubordinationCommand : BaseAdminCommand
    {

        private ModifyAdminSubordinations Model
        {
            get
            {
                if (!(_param is ModifyAdminSubordinations)) throw new WrongParameterTypeError();
                return (ModifyAdminSubordinations)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var row = new InternalAdminSubordination()
                {
                    SourcePositionId = Model.SourcePositionId,
                    SubordinationTypeId = (int)Model.SubordinationTypeId
                };

                CommonDocumentUtilities.SetLastChange(_context, row);

                foreach (var item in Model.TargetPositionIDs)
                {
                    row.TargetPositionId = item;

                    var exists = _adminDb.ExistsSubordination(_context, new FilterAdminSubordination()
                    {
                        SourcePositionIDs = new List<int>() { Model.SourcePositionId },
                        TargetPositionIDs = new List<int>() { item },
                        SubordinationTypeIDs = new List<int>() { (int)Model.SubordinationTypeId }
                    });

                    if (exists && !Model.IsChecked) _adminDb.DeleteSubordination(_context, row);
                    else if (!exists && Model.IsChecked) _adminDb.AddSubordination(_context, row);
                }

                return Model.IsChecked;
                //var model = CommonAdminUtilities.SubordinationModifyToInternal(_context, Model);
                //return _adminDb.AddSubordination(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}