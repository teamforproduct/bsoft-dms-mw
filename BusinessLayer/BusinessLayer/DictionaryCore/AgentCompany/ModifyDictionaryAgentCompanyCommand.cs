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
    public class ModifyDictionaryAgentCompanyCommand : BaseDictionaryAgentCompanyCommand
    {
        public override object Execute()
        {
            try
            {
                var newCompany = new InternalDictionaryAgentCompany(Model); ;
                CommonDocumentUtilities.SetLastChange(_context, newCompany);
                _dictDb.UpdateAgentCompany(_context, newCompany);
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
