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
    public class AddDictionaryAgentAccountCommand : BaseDictionaryCommand
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
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyAgentAccount(_context, _dictDb, Model);
         
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAccount = new InternalDictionaryAgentAccount(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAccount);
                return _dictDb.AddAgentAccount(_context, newAccount);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
