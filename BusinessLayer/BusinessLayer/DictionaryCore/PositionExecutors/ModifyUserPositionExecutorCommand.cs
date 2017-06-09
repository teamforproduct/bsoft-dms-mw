using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class ModifyUserPositionExecutorCommand : BaseUserPositionExecutorCommand
    {
        private ModifyPositionExecutor Model { get { return GetModel<ModifyPositionExecutor>(); } }

        public override object Execute()
        {
            _dictService.ExecuteAction(EnumActions.ModifyExecutor, _context, Model);
            return null;
        }
    }
}