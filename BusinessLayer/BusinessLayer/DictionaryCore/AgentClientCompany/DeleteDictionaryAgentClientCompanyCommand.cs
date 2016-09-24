using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryAgentClientCompanyCommand : BaseDictionaryCommand

    {
      
        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int CompanyId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            try
            {
                _dictDb.DeleteAgentClientCompany(_context, new System.Collections.Generic.List<int>(){ Model });
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

