using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddCustomDictionaryCommand : BaseCustomDictionaryCommand
    {
        private AddCustomDictionary Model { get { return GetModel<AddCustomDictionary>(); } }

        public override object Execute()
        {
            var newItem = new InternalCustomDictionary(Model);
            CommonDocumentUtilities.SetLastChange(_context, newItem);
            return _dictDb.AddCustomDictionary(_context, newItem);
        }
    }
}