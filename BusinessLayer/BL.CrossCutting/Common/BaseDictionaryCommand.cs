using BL.CrossCutting.Interfaces;
using BL.Model.Enums;

namespace BL.CrossCutting.Common
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

        public abstract bool CanBeDisplayed();

        public abstract bool CanExecute();

        public abstract object Execute();

        public abstract EnumDictionaryAction CommandType { get; }
    }
}