using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryContactTypeCommand : BaseDictionaryContactTypeCommand
    {
        private AddContactType Model { get { return GetModel<AddContactType>(); } }

        public override object Execute()
        {
            var newContactType = new InternalDictionaryContactType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newContactType);
            return _dictDb.AddContactType(_context, newContactType);
        }
    }
}
