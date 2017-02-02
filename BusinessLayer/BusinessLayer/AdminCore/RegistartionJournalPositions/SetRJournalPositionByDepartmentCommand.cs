using BL.CrossCutting.DependencyInjection;
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
    public class SetRJournalPositionByDepartmentCommand : BaseRJournalPositionCommand
    {
        private ModifyAdminRegistrationJournalPositionByDepartment Model
        {
            get
            {
                if (!(_param is ModifyAdminRegistrationJournalPositionByDepartment)) throw new WrongParameterTypeError();
                return (ModifyAdminRegistrationJournalPositionByDepartment)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {
                    // Устанавливаю рассылку для должностей заданного отдела
                    SetRegistrationJournalPositionByDepartment(Model.DepartmentId);

                    if (!Model.IgnoreChildDepartments)
                    {
                        // Устанавливаю рассылку для должностей дочерних отделов
                        SetRegistrationJournalPositionByChildDepartments(Model.DepartmentId);
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



        private void SetRegistrationJournalPositionByDepartment(int departmentId)
        {
            var journals = _dictDb.GetRegistrationJournals(_context, new FilterDictionaryRegistrationJournal() { DepartmentIDs = new List<int> { departmentId } }, null);

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

        private void SetRegistrationJournalPositionByChildDepartments(int departmentId)
        {
            var departments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment() { ParentIDs = new List<int> { departmentId } });

            if (departments.Count() > 0)
            {
                foreach (var department in departments)
                {
                    SetRegistrationJournalPositionByDepartment(department.Id);
                    SetRegistrationJournalPositionByChildDepartments(department.Id);
                }
            }
        }
    }
}