using BL.CrossCutting.Interfaces;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface ICommandService
    {
        object ExecuteCommand(ICommand cmd);
    }
}