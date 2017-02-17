using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAddressTypeCommand : BaseDictionaryAddressTypeCommand
    {
        private AddAddressType Model { get { return GetModel<AddAddressType>(); } }

        public override object Execute()
        {
            var newAddrType = new InternalDictionaryAddressType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newAddrType);
            return _dictDb.AddAddressType(_context, newAddrType);
        }
    }
}