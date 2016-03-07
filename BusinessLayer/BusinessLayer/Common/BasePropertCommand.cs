using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.Common
{
    public abstract class BasePropertCommand : IPropertyCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumPropertyAction _action;

        public void InitializeCommand(EnumPropertyAction action, IContext ctx)
        {
            InitializeCommand(action,ctx, null);
        }

        public void InitializeCommand(EnumPropertyAction action, IContext ctx, object param)
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

        public virtual EnumPropertyAction CommandType => _action;
    }
}