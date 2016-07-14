using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Encryption.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Common
{
    public abstract class BaseEncryptionCommand : IEncryptionCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumEncryptionActions _action;
        protected IEncryptionDbProcess _encryptionDb;
        protected IAdminService _admin;

        public void InitializeCommand(EnumEncryptionActions action, IContext ctx)
        {
            InitializeCommand(action,ctx, null);
        }

        public void InitializeCommand(EnumEncryptionActions action, IContext ctx, object param)
        {
            _action = action;
            _context = ctx;
            _param = param;
            _encryptionDb = DmsResolver.Current.Get<IEncryptionDbProcess>();
            _admin = DmsResolver.Current.Get<IAdminService>();
        }

        public InternalDocument Document => null;
        public IContext Context => _context;
        public object Parameters => _param;

        public abstract bool CanBeDisplayed(int positionId);

        public abstract bool CanExecute();

        public abstract object Execute();

        public virtual EnumEncryptionActions CommandType => _action;
    }
}