using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryStandartSendListCommand : BaseDictionaryStandartSendListCommand
    {
        private ModifyStandartSendList Model { get { return GetModel<ModifyStandartSendList>(); } }

        public override object Execute()
        {
            var newList = new InternalDictionaryStandartSendList(Model);
            CommonDocumentUtilities.SetLastChange(_context, newList);
            _dictDb.UpdateStandartSendList(_context, newList);
            return null;
        }
    }
}
