using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryDocumentTypeCommand : BaseDictionaryDocumentTypeCommand
    {
        private ModifyDocumentType Model { get { return GetModel<ModifyDocumentType>(); } }

        public override object Execute()
        {
            var newDocType = new InternalDictionaryDocumentType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newDocType);
            _dictDb.UpdateDocumentType(_context, newDocType);
            return null;
        }
    }
}