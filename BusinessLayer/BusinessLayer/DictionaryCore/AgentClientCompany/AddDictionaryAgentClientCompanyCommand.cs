using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentClientCompanyCommand : BaseDictionaryAgentClientCompanyCommand
    {

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.CompanyModifyToInternal(_context, Model);

                return _dictDb.AddAgentClientCompany(_context, dp);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}