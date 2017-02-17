using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyCustomDictionaryTypeCommand : BaseCustomDictionaryTypeCommand
    {
        private ModifyCustomDictionaryType Model { get { return GetModel<ModifyCustomDictionaryType>(); } }

        public override object Execute()
        {
            var newItem = new InternalCustomDictionaryType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newItem);
            _dictDb.UpdateCustomDictionaryType(_context, newItem);
            return null;
        }
    }
}