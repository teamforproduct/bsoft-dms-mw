using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using System.Collections.Generic;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentCompanyCommand : BaseDictionaryAgentCompanyCommand
    {
        public override object Execute()
        {
            try
            {
                var newCompany = new InternalDictionaryAgentCompany(Model); ;
                CommonDocumentUtilities.SetLastChange(_context, newCompany);
                return _dictDb.AddAgentCompany(_context, newCompany);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
