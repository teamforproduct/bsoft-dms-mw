using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;

namespace BL.Logic.DocumentCore.Commands
{
    public class CommandService : ICommandService
    {
        public object ExecuteCommand(ICommand cmd)
        {
            if (cmd.CanExecute())
            {
                return cmd.Execute();
            }
            return null;
        }
    }
}