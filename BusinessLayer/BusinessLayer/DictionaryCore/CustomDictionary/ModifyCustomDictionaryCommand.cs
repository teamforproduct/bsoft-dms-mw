using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyCustomDictionaryCommand : BaseCustomDictionaryCommand
    {
        private ModifyCustomDictionary Model { get { return GetModel<ModifyCustomDictionary>(); } }

        public override object Execute()
        {
            var newItem = new InternalCustomDictionary(Model);
            CommonDocumentUtilities.SetLastChange(_context, newItem);
            _dictDb.UpdateCustomDictionary(_context, newItem);
            return null;
        }
    }
}