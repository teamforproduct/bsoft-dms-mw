using BL.CrossCutting.Interfaces;

namespace BL.CrossCutting.Common
{
    public abstract class Command
    {
        public abstract object Execute(object parameter);
        public abstract bool CanExecute();
        public abstract object Execute();
    }
}