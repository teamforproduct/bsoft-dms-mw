using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using System.Transactions;
using BL.Model.Enums;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryAgentCompanyCommand : BaseDictionaryCommand
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {

            _adminService.VerifyAccess(_context, CommandType, false, true);
            return true;
        }

        public override object Execute()
        {
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {
                    var frontObj = _dictService.GetAgentCompany(_context, Model); ;
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentCompanies, (int)CommandType, frontObj.Id, frontObj);

                    _dictDb.DeleteAgentCompanies(_context, new System.Collections.Generic.List<int>() { Model });

                    transaction.Complete();
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }
}
