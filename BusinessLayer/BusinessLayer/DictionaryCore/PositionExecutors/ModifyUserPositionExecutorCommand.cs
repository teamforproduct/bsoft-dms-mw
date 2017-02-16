using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class ModifyUserPositionExecutorCommand : BaseDictionaryPositionExecutorCommand
    {
        private ModifyPositionExecutor Model { get { return GetModel<ModifyPositionExecutor>(); } }

        public override object Execute()
        {
            _dictService.ExecuteAction(EnumDictionaryActions.ModifyExecutor, _context, Model);
            return null;
        }
    }
}