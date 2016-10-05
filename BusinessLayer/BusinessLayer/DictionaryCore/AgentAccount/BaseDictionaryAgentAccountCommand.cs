using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAgentAccountCommand : BaseDictionaryCommand
    {
        protected ModifyDictionaryAgentAccount Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentAccount))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentAccount)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.AccountNumber?.Trim();

            var spr = _dictDb.GetAgentAccounts(_context, new FilterDictionaryAgentAccount
            {
                AgentId = Model.AgentId,
                AccountNumber = Model.AccountNumber,
                NotContainsIDs = new List<int> { Model.Id }
            });

            if (spr.Count() != 0)
            {
                throw new DictionaryAgentAccountNumberNotUnique(Model.AccountNumber);
            }

            return true;
        }

        public override object Execute()
        {
            if (Model.IsMain) SetMainAgentAccount();
            return null;
        }

        private void SetMainAgentAccount()
        {
            var accounts = _dictDb.GetInternalAgentAccounts(_context, new FilterDictionaryAgentAccount() { AgentId = Model.AgentId });

            foreach (InternalDictionaryAgentAccount account in accounts)
            {
                if (account.Id != Model.Id)
                {
                    account.IsMain = false;
                    _dictDb.UpdateAgentAccount(_context, account);
                }
            }
        }
    }
}
