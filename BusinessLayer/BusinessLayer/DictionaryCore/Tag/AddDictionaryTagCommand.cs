using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryTagCommand : BaseDictionaryTagCommand
    {
        private AddTag Model { get { return GetModel<AddTag>(); } }

        public override object Execute()
        {
            var item = new InternalDictionaryTag(Model);
            CommonDocumentUtilities.SetLastChange(_context, item);
            return _dictDb.AddTag(_context, item);
        }
    }
}