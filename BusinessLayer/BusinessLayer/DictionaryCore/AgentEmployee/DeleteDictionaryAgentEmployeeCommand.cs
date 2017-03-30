using BL.Logic.Common;
using System.Collections.Generic;

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

            _dictDb.DeleteAgentUser(_context, Model);
            _dictDb.DeleteAgentPeople(_context, Model);
            _dictService.DeleteAgentIfNoAny(_context, new List<int>() { Model });

            return null;
        }
    }
}
