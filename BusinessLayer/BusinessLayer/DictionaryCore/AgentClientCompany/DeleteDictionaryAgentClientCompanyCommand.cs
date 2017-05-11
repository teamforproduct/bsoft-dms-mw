using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryAgentClientCompanyCommand : BaseDictionaryCommand

    {

        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var frontObj = _dictDb.GetAgentOrgs(_context, new FilterDictionaryAgentOrg { IDs = new List<int> { Model } }).FirstOrDefault();
                if (frontObj != null) _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentClientCompanies, (int)CommandType, frontObj.Id, frontObj);

                // ссылку на компанию имеют все отделы, любого уровня вложенности
                var departments = _dictDb.GetDepartmentIDs(_context, new FilterDictionaryDepartment() { CompanyIDs = new List<int> { Model } });

                if (departments.Count > 0) _dictService.DeleteDepartments(_context, departments, false);

                _dictDb.DeleteAgentOrg(_context, new FilterDictionaryAgentOrg { IDs = new List<int>() { Model } });

                _dictService.DeleteAgentIfNoAny(_context, new List<int>() { Model });

                transaction.Complete();
                return null;
            }
        }
    }

}

