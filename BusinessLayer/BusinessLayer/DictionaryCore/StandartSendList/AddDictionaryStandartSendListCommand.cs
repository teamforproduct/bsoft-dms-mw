using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryStandartSendListCommand : BaseDictionaryStandartSendListCommand
    {
        private AddStandartSendList Model { get { return GetModel<AddStandartSendList>(); } }

        public override object Execute()
        {
            var newList = new InternalDictionaryStandartSendList()
            {
                Name = Model.Name,
                PositionId = Model.PositionId

            };
            CommonDocumentUtilities.SetLastChange(_context, newList);
            return _dictDb.AddStandartSendList(_context, newList);
        }
    }
}
