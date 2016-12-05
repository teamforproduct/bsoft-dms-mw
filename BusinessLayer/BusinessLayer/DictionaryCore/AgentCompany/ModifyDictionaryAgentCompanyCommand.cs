using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BL.Model.Enums;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentCompanyCommand : BaseDictionaryAgentCompanyCommand
    {
        public override object Execute()
        {
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {
                    var newCompany = new InternalDictionaryAgentCompany(Model); ;
                    CommonDocumentUtilities.SetLastChange(_context, newCompany);
                    _dictDb.UpdateAgentCompany(_context, newCompany);
                    var frontObj = _dictDb.GetAgentCompany(_context, Model.Id); ;
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentCompanies, (int)CommandType, frontObj.Id, frontObj);

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
