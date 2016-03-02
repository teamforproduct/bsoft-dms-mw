using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Common
{
    public abstract class BaseDocumentCommand: IDocumentCommand
    {
        protected IContext _context;
        protected InternalDocument _document;
        protected object _param;
        protected EnumDocumentActions _action;

        public void InitializeCommand(IContext ctx, InternalDocument doc)
        {
            InitializeCommand(ctx, doc, null, null);
        }

        public void InitializeCommand(IContext ctx, InternalDocument doc, object param, EnumDocumentActions? action)
        {
            _context = ctx;
            _document = doc;
            _param = param;
            _action = action?? EnumDocumentActions.Undefined;
        }

        public InternalDocument Document { get { return _document; } }
        public IContext Context { get { return _context; } }
        public object Parameters { get { return _param; } }

        public abstract bool CanBeDisplayed();
        public abstract bool CanExecute();
        public abstract object Execute();

        public virtual EnumDocumentActions CommandType { get { return _action; } }
    }
}