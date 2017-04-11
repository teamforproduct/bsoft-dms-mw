using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    class DeleteDictionaryAgentEmployeeCommand : BaseDictionaryCommand
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
                _dictDb.DeleteAgentEmployees(_context, new FilterDictionaryAgentEmployee { IDs = new List<int> { Model } });

                _dictDb.DeleteAgentUsers(_context, new FilterDictionaryAgentUsers { IDs = new List<int> { Model } });
                _dictDb.DeleteAgentPeoples(_context, new FilterDictionaryAgentPeoples { IDs = new List<int> { Model } });
                _dictService.DeleteAgentIfNoAny(_context, new List<int>() { Model });

                transaction.Complete();
            }
            return null;
        }
    }
}
