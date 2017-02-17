using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentBankCommand : BaseDictionaryAgentBankCommand
    {
        private AddAgentBank Model { get { return GetModel<AddAgentBank>(); } }

        public override object Execute()
        {
            var newBank = new InternalDictionaryAgentBank(Model);
            CommonDocumentUtilities.SetLastChange(_context, newBank);
            return _dictDb.AddAgentBank(_context, newBank);
        }
    }
}
