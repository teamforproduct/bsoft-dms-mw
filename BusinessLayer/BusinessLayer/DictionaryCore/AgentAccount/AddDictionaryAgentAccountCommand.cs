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
    public class AddDictionaryAgentAccountCommand : BaseDictionaryAgentAccountCommand
    {
        public override object Execute()
        {
            try
            {
                var newAccount = new InternalDictionaryAgentAccount(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAccount);
                int account = _dictDb.AddAgentAccount(_context, newAccount);

                base.Execute();

                return account;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
