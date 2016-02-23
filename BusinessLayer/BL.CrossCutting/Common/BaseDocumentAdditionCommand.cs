using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.CrossCutting.Common
{
    public abstract class BaseDocumentAdditionCommand: IDocumentAdditionCommand
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

        public InternalDocument Document { get { return _document; } }
        public IContext Context { get { return _context; } }
        public object Parameters { get { return _param; } }

        public abstract bool CanBeDisplayed();
        public abstract bool CanExecute();
        public abstract object Execute();

        public abstract EnumDocumentAdditionActions CommandType { get; }
    }
}