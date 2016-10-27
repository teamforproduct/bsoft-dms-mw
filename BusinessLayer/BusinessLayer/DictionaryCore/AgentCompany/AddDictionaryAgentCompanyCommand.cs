﻿using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using System.Collections.Generic;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Linq;
using BL.Model.Enums;
using System.Transactions;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentCompanyCommand : BaseDictionaryAgentCompanyCommand
    {
        public override object Execute()
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    var newCompany = new InternalDictionaryAgentCompany(Model); ;
                    CommonDocumentUtilities.SetLastChange(_context, newCompany);
                    var id = _dictDb.AddAgentCompany(_context, newCompany);
                    var frontObj = _dictDb.GetAgentCompany(_context, id);
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentCompanies, (int)CommandType, frontObj.Id, frontObj);

                    transaction.Complete();

                    return id;
                }
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
