using BL.Model.DocumentCore.InternalModel;

namespace BL.CrossCutting.Interfaces
{
    public interface ICommand
    {
        void InitializeCommand(IContext ctx, InternalDocument doc);
        void InitializeCommand(IContext ctx, InternalDocument doc, object param);
        bool CanBeDisplayed();
        bool CanExecute();
        object Execute();
    }
}