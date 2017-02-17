using BL.Logic.Common;


namespace BL.Logic.DictionaryCore
{
    class DeleteDictionaryAgentEmployeeCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);
            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteAgentEmployee(_context, Model);
            return null;
        }
    }
}
