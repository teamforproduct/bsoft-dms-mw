using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryContactTypeCommand : BaseDictionaryContactTypeCommand
    {
        private ModifyContactType Model { get { return GetModel<ModifyContactType>(); } }

        public override object Execute()
        {
            var newContactType = new InternalDictionaryContactType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newContactType);
            _dictDb.UpdateContactType(_context, newContactType);
            return null;
        }
    }
}
