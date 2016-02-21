using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;

namespace BL.CrossCutting.Common
{
    public abstract class BaseCommand:ICommand
    {
        protected IContext _context;
        protected InternalDocument _document;
        protected object _param;

        public void InitializeCommand(IContext ctx, InternalDocument doc)
        {
            InitializeCommand(ctx, doc, null);
        }

        public void InitializeCommand(IContext ctx, InternalDocument doc, object param)
        {
            _context = ctx;
            _document = doc;
            _param = param;
        }

        public abstract bool CanBeDisplayed();
        public abstract bool CanExecute();
        public abstract object Execute();

    }
}