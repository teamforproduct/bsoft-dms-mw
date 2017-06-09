using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.Common
{
    public abstract class BasePropertyCommand : IPropertyCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumActions _action;

        public void InitializeCommand(EnumActions action, IContext ctx)
        {
            InitializeCommand(action,ctx, null);
        }

        public void InitializeCommand(EnumActions action, IContext ctx, object param)
        {
            _action = action;
            _context = ctx;
            _param = param;
        }

        public InternalDocument Document { get { return null; } }
        public IContext Context { get { return _context; } }
        public object Parameters { get { return _param; } }

        public abstract bool CanBeDisplayed(int positionId);

        public abstract bool CanExecute();

        public abstract object Execute();

        public virtual EnumActions CommandType => _action;

        public bool TypeModelIs<T>() => _param is T;

        public T GetModel<T>()
        {
            if (!(TypeModelIs<T>()))
            {
                throw new WrongParameterTypeError();
            }
            return (T)_param;
        }
    }
}