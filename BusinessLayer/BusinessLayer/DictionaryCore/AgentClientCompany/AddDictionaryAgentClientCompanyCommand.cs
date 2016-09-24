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

            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyAgentClientCompany(_context, _dictDb, Model);

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