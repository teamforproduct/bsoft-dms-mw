using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryStandartSendListContentCommand : BaseDictionaryStandartSendListContentCommand
    {
        private AddStandartSendListContent Model { get { return GetModel<AddStandartSendListContent>(); } }

        public override object Execute()
        {
            var newCont = new InternalDictionaryStandartSendListContent(Model);
            CommonDocumentUtilities.SetLastChange(_context, newCont);
            return _dictDb.AddStandartSendListContent(_context, newCont);
        }
    }
}
