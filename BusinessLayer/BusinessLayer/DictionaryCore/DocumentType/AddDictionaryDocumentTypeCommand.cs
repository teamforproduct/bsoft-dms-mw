using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryDocumentTypeCommand : BaseDictionaryDocumentTypeCommand
    {
        private AddDocumentType Model { get { return GetModel<AddDocumentType>(); } }

        public override object Execute()
        {
            var newDocType = new InternalDictionaryDocumentType(Model);
            CommonDocumentUtilities.SetLastChange(_context, newDocType);
            return _dictDb.AddDocumentType(_context, newDocType);
        }
    }
}