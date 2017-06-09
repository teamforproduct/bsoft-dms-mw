using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Constants;
using BL.Logic.DocumentCore.Interfaces;

namespace BL.Logic.Common
{
    public abstract class BaseDocumentCommand: IDocumentCommand
    {
        protected IContext _context;
        protected InternalDocument _document;
        protected object _param;
        protected EnumActions _action;
        protected IEnumerable<InternalActionRecord> _actionRecords;
        protected IAdminService _adminProc;
        protected IDocumentService _documentProc;
        protected ILogger _logger;

        public void InitializeCommand(IContext ctx, InternalDocument doc)
        {
            InitializeCommand(ctx, doc, null, null);
        }

        public void InitializeCommand(IContext ctx, InternalDocument doc, object param, EnumActions? action)
        {
            _context = ctx;
            _document = doc;
            _param = param;
            _action = action?? EnumActions.Undefined;
            _adminProc = DmsResolver.Current.Get<IAdminService>();
            _documentProc = DmsResolver.Current.Get<IDocumentService>();
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        public InternalDocument Document => _document;
        public IContext Context => _context;
        public object Parameters => _param;
        public IEnumerable<InternalActionRecord> ActionRecords => _actionRecords;

        public abstract bool CanBeDisplayed(int positionId);
        public abstract bool CanExecute();
        public abstract object Execute();

        public virtual EnumActions CommandType => _action;

        protected bool GetIsUseInternalSign()
        {
            var sett = DmsResolver.Current.Get<ISettingValues>();
            return sett.GetDigitalSignatureIsUseInternalSign(_context);
        }
        protected bool GetIsUseCertificateSign()
        {
            var sett = DmsResolver.Current.Get<ISettingValues>();
            return sett.GetDigitalSignatureIsUseCertificateSign(_context);
        }
    }
}