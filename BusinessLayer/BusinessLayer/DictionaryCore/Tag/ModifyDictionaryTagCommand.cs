using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryTagCommand : BaseDictionaryTagCommand
    {
        private ModifyTag Model { get { return GetModel<ModifyTag>(); } }
        public override object Execute()
        {
            var item = new InternalDictionaryTag(Model);
            CommonDocumentUtilities.SetLastChange(_context, item);
            _dictDb.UpdateTag(_context, item);
            return null;
        }
    }
}