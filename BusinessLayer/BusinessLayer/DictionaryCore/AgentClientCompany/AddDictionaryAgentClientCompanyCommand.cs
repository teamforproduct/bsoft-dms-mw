using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentClientCompanyCommand : BaseDictionaryAgentClientCompanyCommand
    {

        private AddAgentClientCompany Model { get { return GetModel<AddAgentClientCompany>(); } }

        public override object Execute()
        {
            try
            {
                using (var transaction = Transactions.GetTransaction())
                {

                    var model = new InternalDictionaryAgentClientCompany(Model);

                    CommonDocumentUtilities.SetLastChange(_context, model);

                    var id = _dictDb.AddAgentClientCompany(_context, model);

                    var frontObj = _dictDb.GetAgentClientCompanies(_context, new FilterDictionaryAgentClientCompany { IDs = new List<int> { id } }).FirstOrDefault();
                    _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentClientCompanies, (int)CommandType, frontObj.Id, frontObj);

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