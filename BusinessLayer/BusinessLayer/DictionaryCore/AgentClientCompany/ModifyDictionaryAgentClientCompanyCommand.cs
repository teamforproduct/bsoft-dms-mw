using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentClientCompanyCommand : BaseDictionaryCommand
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

                _dictDb.UpdateAgentClientCompany(_context, dp);
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