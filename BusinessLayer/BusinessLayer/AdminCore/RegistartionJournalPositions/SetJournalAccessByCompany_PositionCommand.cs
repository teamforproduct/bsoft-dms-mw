using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessByCompany_PositionCommand : BaseJournalAccessCommand
    {
        private SetJournalAccessByCompany_Position Model { get { return GetModel<SetJournalAccessByCompany_Position>(); } }

        public override object Execute()
        {
            try
            {
                var departments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { CompanyIDs = new List<int> { Model.CompanyId }, IsActive = true });

                using (var transaction = Transactions.GetTransaction())
                {
                    if (departments.Count() > 0)
                    {
                        foreach (var department in departments)
                        {
                            SetByDepartment_Position(department.Id, Model.PositionId, Model.IsChecked, Model.RegJournalAccessTypeId);
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


        

    }
}