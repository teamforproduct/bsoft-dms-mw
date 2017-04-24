using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Constants;

namespace BL.Logic.Common
{
    public abstract class BaseDocumentCommand: IDocumentCommand
    {
        protected IContext _context;
        protected InternalDocument _document;
        protected object _param;
        protected EnumDocumentActions _action;
        protected IEnumerable<InternalActionRecord> _actionRecords;
        protected IAdminService _admin;
        protected ILogger _logger;

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
            _admin = DmsResolver.Current.Get<IAdminService>();
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        public InternalDocument Document => _document;
        public IContext Context => _context;
        public object Parameters => _param;
        public IEnumerable<InternalActionRecord> ActionRecords => _actionRecords;

        public abstract bool CanBeDisplayed(int positionId);
        public abstract bool CanExecute();
        public abstract object Execute();

        public virtual EnumDocumentActions CommandType => _action;

        protected bool GetIsUseInternalSign()
        {
            var sett = DmsResolver.Current.Get<ISettingValues>();
            return sett.GetDigitalSignatureIsUseInternalSign(_context);
            //try
            //{
            //    return sett.GetSetting<bool>(_context, SettingConstants.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN);
            //}
            //catch
            //{
            //    sett.SaveSetting(_context, SettingConstants.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN, false);
            //    return sett.GetSetting<bool>(_context, SettingConstants.DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN);
            //}
        }
        protected bool GetIsUseCertificateSign()
        {
            var sett = DmsResolver.Current.Get<ISettingValues>();
            return sett.GetDigitalSignatureIsUseCertificateSign(_context);
            //try
            //{
            //    return sett.GetSetting<bool>(_context, SettingConstants.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN);
            //}
            //catch
            //{
            //    sett.SaveSetting(_context, SettingConstants.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN, false);
            //    return sett.GetSetting<bool>(_context, SettingConstants.DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN);
            //}
        }
    }
}