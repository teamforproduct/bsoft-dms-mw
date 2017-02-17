using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentAddressCommand : BaseDictionaryAgentAddressCommand
    {
        private ModifyAgentAddress Model { get { return GetModel<ModifyAgentAddress>(); } }

        public override object Execute()
        {
            var newAddr = new InternalDictionaryAgentAddress(Model);
            CommonDocumentUtilities.SetLastChange(_context, newAddr);
            _dictDb.UpdateAgentAddress(_context, newAddr);
            return null;
        }
    }
}
