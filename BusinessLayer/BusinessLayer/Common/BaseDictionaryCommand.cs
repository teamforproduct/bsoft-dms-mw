using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore;

namespace BL.Logic.Common
{
    public abstract class BaseDictionaryCommand: IDictionaryCommand
    {
        protected IContext _context;
        protected object _param;

        public void InitializeCommand(IContext ctx)
        {
            InitializeCommand(ctx, null);
        }

        public void InitializeCommand(IContext ctx, object param)
        {
            _context = ctx;
            _param = param;
        }

        public InternalDocument Document { get { return null; } }
        public IContext Context { get { return _context; } }
        public object Parameters { get { return _param; } }

        public abstract bool CanBeDisplayed(int positionId, InternalSystemAction action);

        public abstract bool CanExecute();

        public abstract object Execute();

        public abstract EnumDictionaryAction CommandType { get; }
    }
}