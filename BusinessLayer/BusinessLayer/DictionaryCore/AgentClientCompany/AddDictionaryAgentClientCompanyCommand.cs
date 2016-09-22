using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentClientCompanyCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryAgentClientCompany Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentClientCompany))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentClientCompany)_param;
            }
        }

        public override bool CanBeDisplayed(int CompanyId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType, false);

            if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent() { NameExact = Model.Name }))
            {
                throw new DictionaryAgentNameNotUnique();
            }

            return true;
        }

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