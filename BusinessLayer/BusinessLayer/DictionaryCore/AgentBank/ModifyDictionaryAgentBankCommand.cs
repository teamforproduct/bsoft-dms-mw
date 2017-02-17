using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentBankCommand : BaseDictionaryAgentBankCommand
    {
        private ModifyAgentBank Model { get { return GetModel<ModifyAgentBank>(); } }

        public override object Execute()
        {
            var newBank = new InternalDictionaryAgentBank(Model);
            CommonDocumentUtilities.SetLastChange(_context, newBank);
            _dictDb.UpdateAgentBank(_context, newBank);
            return null;
        }
    }
}
