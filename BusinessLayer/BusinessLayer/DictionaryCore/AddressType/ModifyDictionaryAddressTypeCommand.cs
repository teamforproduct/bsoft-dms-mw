using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAddressTypeCommand : BaseDictionaryAddressTypeCommand
    {
        private ModifyAddressType Model { get { return GetModel<ModifyAddressType>(); } }

        public override object Execute()
        {
            var newAddrType = new InternalDictionaryAddressType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newAddrType);
            _dictDb.UpdateAddressType(_context, newAddrType);
            return null;
        }
    }
}