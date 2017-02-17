using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentCompanyCommand : BaseDictionaryAgentCompanyCommand
    {
        private ModifyAgentCompany Model { get { return GetModel<ModifyAgentCompany>(); } }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var newCompany = new InternalDictionaryAgentCompany(Model); ;
                CommonDocumentUtilities.SetLastChange(_context, newCompany);
                _dictDb.UpdateAgentCompany(_context, newCompany);
                var frontObj = _dictService.GetAgentCompany(_context, Model.Id); ;
                _logger.Information(_context, null, (int)EnumObjects.DictionaryAgentCompanies, (int)CommandType, frontObj.Id, frontObj);

                transaction.Complete();
            }
            return null;
        }
    }
}
