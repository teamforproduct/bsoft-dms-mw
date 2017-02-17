using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    class AddDictionaryAgentAddressCommand : BaseDictionaryAgentAddressCommand
    {
        private AddAgentAddress Model { get { return GetModel<AddAgentAddress>(); } }

        public override object Execute()
        {
            var newAddr = new InternalDictionaryAgentAddress(Model);
            CommonDocumentUtilities.SetLastChange(_context, newAddr);
            return _dictDb.AddAgentAddress(_context, newAddr);
        }
    }
}
