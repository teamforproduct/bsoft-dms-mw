using BL.CrossCutting.Helpers;
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
    public class SetRJournalPositionByCompanyCommand : BaseRJournalPositionCommand
    {
        private ModifyAdminRegistrationJournalPositionByCompany Model
        {
            get
            {
                if (!(_param is ModifyAdminRegistrationJournalPositionByCompany)) throw new WrongParameterTypeError();
                return (ModifyAdminRegistrationJournalPositionByCompany)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                var departments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { CompanyIDs = new List<int> { Model.CompanyId } });
                // PSS ReadUncommitted - так как вложенная транзакция ReadUncommitted
                using (var transaction = Transactions.GetTransaction())
                {
                    if (departments.Count() > 0)
                    {
                        foreach (var department in departments)
                        {
                            SetRJournalPositionByDepartment(department.Id);
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


        private void SetRJournalPositionByDepartment(int departmentId)
        {
            var journals = _dictDb.GetRegistrationJournals(_context, new FilterDictionaryRegistrationJournal() { DepartmentIDs = new List<int> { departmentId } });

            if (journals.Count() > 0)
            {
                foreach (var journal in journals)
                {
                    SetRegistrationJournalPosition(new ModifyAdminRegistrationJournalPosition()
                    {
                        IsChecked = Model.IsChecked,
                        PositionId = Model.PositionId,
                        RegistrationJournalId = journal.Id,
                        RegJournalAccessTypeId = Model.RegJournalAccessTypeId
                    });
                }
            }
        }

    }
}