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
    public class ModifyDictionaryAgentAccountCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentAccount Model
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

       
            _admin.VerifyAccess(_context, CommandType, false);
            var spr = _dictDb.GetAgentAccounts(_context, Model.AgentId, new FilterDictionaryAgentAccount
            {
                AgentBankId = Model.AgentBankId,
                AccountNumber = Model.AccountNumber
            });
            if (spr.Count() != 0)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAccount = new InternalDictionaryAgentAccount(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAccount);
                _dictDb.UpdateAgentAccount(_context, newAccount);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}
