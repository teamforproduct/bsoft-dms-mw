using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryStandartSendListContentCommand : BaseDictionaryStandartSendListContentCommand
    {
        private ModifyStandartSendListContent Model { get { return GetModel<ModifyStandartSendListContent>(); } }

        public override object Execute()
        {
            var model = new InternalDictionaryStandartSendListContent(Model);
            CommonDocumentUtilities.SetLastChange(_context, model);
            _dictDb.UpdateStandartSendListContent(_context, model);
            return null;
        }
    }
}
