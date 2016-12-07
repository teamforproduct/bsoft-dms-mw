using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;
using System.Transactions;
using BL.Model.Enums;
using System.Linq;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentClientCompanyCommand : BaseDictionaryAgentClientCompanyCommand
    {
        public override object Execute()
        {
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {

                    var dp = CommonDictionaryUtilities.CompanyModifyToInternal(_context, Model);

                    _dictDb.UpdateAgentClientCompany(_context, dp);

                    var frontObj = _dictDb.GetAgentClientCompanies(_context, new FilterDictionaryAgentClientCompany { IDs = new List<int> { dp.Id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentClientCompanies, (int)CommandType, frontObj.Id, frontObj);

                    transaction.Complete();
                }
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