using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;

namespace BL.CrossCutting.Interfaces
{
    public interface ICommand
    {
        InternalDocument Document { get; }
        IContext Context { get; }
        object Parameters { get; }

        bool CanBeDisplayed(int positionId, InternalSystemAction action);
        bool CanExecute();
        object Execute();
        
    }
}