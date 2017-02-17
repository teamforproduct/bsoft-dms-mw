using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddCustomDictionaryTypeCommand : BaseCustomDictionaryTypeCommand
    {
        private AddCustomDictionaryType Model { get { return GetModel<AddCustomDictionaryType>(); } }

        public override object Execute()
        {
            var newItem = new InternalCustomDictionaryType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newItem);
            return _dictDb.AddCustomDictionaryType(_context, newItem);
        }
    }
}