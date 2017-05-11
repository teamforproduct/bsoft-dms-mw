using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentClientCompanyCommand : BaseDictionaryAgentClientCompanyCommand
    {
        private ModifyOrg Model { get { return GetModel<ModifyOrg>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {

                var model = new InternalDictionaryAgentOrg(Model);
                CommonDocumentUtilities.SetLastChange(_context, model);
                _dictDb.UpdateAgentOrg(_context, model);

                var frontObj = _dictDb.GetAgentOrgs(_context, new FilterDictionaryAgentOrg { IDs = new List<int> { model.Id } }).FirstOrDefault();
                if (frontObj != null) _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentClientCompanies, (int)CommandType, frontObj.Id, frontObj);

                transaction.Complete();
            }
            return null;
        }
    }
}