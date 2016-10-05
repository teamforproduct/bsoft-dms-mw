using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

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

        public override object Execute()
        {
            try
            {
                var departments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { CompanyIDs = new List<int> { Model.CompanyId } });

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (departments.Count() > 0)
                    {
                        foreach (var department in departments)
                        {
                            SetSubordinationByDepartment(department.Id);
                        }
                    }
                    transaction.Complete();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }

        private void SetSubordination(ModifyAdminSubordination model)
        {
            _adminService.ExecuteAction(BL.Model.Enums.EnumAdminActions.SetSubordination, _context, model);
        }

        private void SetSubordinationByDepartment(int departmentId)
        {
            var positions = _dictDb.GetPositions(_context, new FilterDictionaryPosition() { DepartmentIDs = new List<int> { departmentId } });

            if (positions.Count() > 0)
            {
                foreach (var position in positions)
                {
                    SetSubordination(new ModifyAdminSubordination()
                    {
                        IsChecked = Model.IsChecked,
                        SourcePositionId = Model.SourcePositionId,
                        TargetPositionId = position.Id,
                        SubordinationTypeId = Model.SubordinationTypeId
                    });
                }
            }
        }

    }
}