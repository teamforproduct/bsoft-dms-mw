namespace BL.CrossCutting.Interfaces
{
    public interface ICommand
    {
        bool CanBeDisplayed();
        bool CanExecute();
        object Execute();
        
    }
}