using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentCompanyCommand : BaseDictionaryAgentCompanyCommand
    {
        private AddAgentCompany Model { get { return GetModel<AddAgentCompany>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {

                var newCompany = new InternalDictionaryAgentCompany(Model); ;
                CommonDocumentUtilities.SetLastChange(_context, newCompany);
                var id = _dictDb.AddAgentCompany(_context, newCompany);
                var frontObj = _dictService.GetAgentCompany(_context, id);
                if (frontObj != null) _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentCompanies, (int)CommandType, frontObj.Id, frontObj);

                transaction.Complete();

                return id;
            }
        }
    }
}
