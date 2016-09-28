using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetSubordinationByCompanyCommand : BaseSubordinationCommand
    {
        private ModifyAdminSubordinationByCompany Model
        {
            get
            {
                if (!(_param is ModifyAdminSubordinationByCompany)) throw new WrongParameterTypeError();
                return (ModifyAdminSubordinationByCompany)_param;
            }
        }

        public override object Execute
        {
            get
            {
                try
                {
                    var row = new InternalAdminSubordination()
                    {
                        SourcePositionId = Model.SourcePositionId,
                        //TargetPositionId = Model.TargetPositionId,
                        SubordinationTypeId = (int)Model.SubordinationTypeId
                    };

                    CommonDocumentUtilities.SetLastChange(_context, row);

                    var exists = _adminDb.ExistsSubordination(_context, new FilterAdminSubordination()
                    {
                        SourcePositionIDs = new List<int>() { row.SourcePositionId },
                        TargetPositionIDs = new List<int>() { row.TargetPositionId },
                        SubordinationTypeIDs = new List<int>() { row.SubordinationTypeId }
                    });

                    if (exists && !Model.IsChecked) _adminDb.DeleteSubordination(_context, row);
                    else if (!exists && Model.IsChecked) _adminDb.AddSubordination(_context, row);

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
}