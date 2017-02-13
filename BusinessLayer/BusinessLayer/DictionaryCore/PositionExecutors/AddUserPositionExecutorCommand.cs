using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class AddUserPositionExecutorCommand : BaseUserPositionExecutorCommand
    {
        private AddPositionExecutor Model { get { return GetModel<AddPositionExecutor>(); } }

        public override object Execute()
        {
            _dictService.ExecuteAction(EnumDictionaryActions.AddExecutor, _context, Model);
            return null;
        }
    }
}