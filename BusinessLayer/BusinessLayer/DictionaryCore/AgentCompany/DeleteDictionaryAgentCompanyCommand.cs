using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryAgentCompanyCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {

            _adminService.VerifyAccess(_context, CommandType, false, true);
            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var frontObj = _dictService.GetAgentCompany(_context, Model); ;
                if (frontObj != null) _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentCompanies, (int)CommandType, frontObj.Id, frontObj);

                var persons = _dictDb.GetInternalAgentPersons(_context, new BL.Model.DictionaryCore.FilterModel.FilterDictionaryAgentPerson { CompanyIDs = new System.Collections.Generic.List<int> { Model } });

                foreach (var item in persons)
                {
                    _dictService.DeleteAgentPerson(_context, item.Id);
                }

                _dictDb.DeleteAgentCompanies(_context, new FilterDictionaryAgentCompany { IDs = new List<int>() { Model } } );
                _dictService.DeleteAgentIfNoAny(_context, new List<int>() { Model });

                transaction.Complete();
                return null;
            }

        }
    }
}
