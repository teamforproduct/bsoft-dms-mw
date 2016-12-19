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
    public class ModifyDictionaryAgentBankCommand : BaseDictionaryAgentBankCommand
    {
        private ModifyAgentBank Model { get { return GetModel<ModifyAgentBank>(); } }

        public override object Execute()
        {
            try
            {
                var newBank = new InternalDictionaryAgentBank(Model);
                CommonDocumentUtilities.SetLastChange(_context, newBank);
                _dictDb.UpdateAgentBank(_context, newBank);
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
