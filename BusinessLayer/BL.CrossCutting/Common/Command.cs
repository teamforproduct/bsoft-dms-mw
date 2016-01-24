using BL.CrossCutting.Interfaces;

namespace BL.CrossCutting.Common
{
    public abstract class Command
    {
        public abstract void Execute();
        public abstract void Execute(object parameter);
        public abstract bool CanExecute();

    }
}