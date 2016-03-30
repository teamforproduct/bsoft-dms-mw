using BL.Model.DocumentCore.InternalModel;


namespace BL.CrossCutting.Interfaces
{
    public interface ICommand
    {
        InternalDocument Document { get; }
        IContext Context { get; }
        object Parameters { get; }

        bool CanBeDisplayed(int positionId);
        bool CanExecute();
        object Execute();
        
    }
}