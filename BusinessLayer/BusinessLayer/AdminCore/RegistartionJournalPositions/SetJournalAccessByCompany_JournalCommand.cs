using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessByCompany_JournalCommand : BaseJournalAccessCommand
    {
        private SetJournalAccessByCompany_Journal Model { get { return GetModel<SetJournalAccessByCompany_Journal>(); } }

        public override object Execute()
        {
            var departments = _dictDb.GetDepartments(_context, new FilterDictionaryDepartment { CompanyIDs = new List<int> { Model.CompanyId }, IsActive = true });

            using (var transaction = Transactions.GetTransaction())
            {
                if (departments.Count() > 0)
                {
                    foreach (var department in departments)
                    {
                        SetByDepartment_Journal(department.Id, Model.JournalId, Model.IsChecked, Model.RegJournalAccessTypeId);
                    }
                }
                transaction.Complete();
            }

            return null;
        }

    }
}