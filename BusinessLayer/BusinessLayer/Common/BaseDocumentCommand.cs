using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System.Collections.Generic;

namespace BL.Logic.Common
{
    public abstract class BaseDocumentCommand: IDocumentCommand
    {
        protected IContext _context;
        protected InternalDocument _document;
        protected object _param;
        protected EnumDocumentActions _action;
        protected IEnumerable<InternalActionRecord> _actionRecords;

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

        public InternalDocument Document => _document;
        public IContext Context => _context;
        public object Parameters => _param;
        public IEnumerable<InternalActionRecord> ActionRecords => _actionRecords;

        public abstract bool CanBeDisplayed(int positionId);
        public abstract bool CanExecute();
        public abstract object Execute();

        public virtual EnumDocumentActions CommandType => _action;
    }
}