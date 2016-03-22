using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore.AgentAccount
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

            _admin.VerifyAccess(_context, CommandType, false, true);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAccount = new InternalDictionaryAgentAccount
                {
                    Id = Model.Id,
                    AgentId = Model.AgentId,
                    AgentBankId = Model.AgentBankId,
                    Name = Model.Name,
                    AccountNumber = Model.AccountNumber,
                    IsMain = Model.IsMain,
                    Description = Model.Description,
                    IsActive = Model.IsActive
                };

                CommonDocumentUtilities.SetLastChange(_context, newAccount);
                _dictDb.UpdateDictionaryAgentAccount(_context, newAccount);
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
