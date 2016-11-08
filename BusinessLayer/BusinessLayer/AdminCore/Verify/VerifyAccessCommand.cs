using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.AdminCore.Verify
{
    public class VerifyAccessCommand : BaseAdminCommand
    {
        protected int Model
        {
            get
            {
                if (!(_param is int)) throw new WrongParameterTypeError();
                return (int)_param;
            }
        }

        private InternalDictionaryAgentUser AgentUser { get; set; }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            AgentUser = _dictDb.GetInternalAgentUser(_context, Model);

            if (AgentUser == null)
                throw new WrongParameterTypeError();

            return true;
        }

        public override object Execute()
        {
            if (AgentUser == null)
                throw new WrongParameterTypeError();

            return AgentUser.UserId;
        }
    }
}
