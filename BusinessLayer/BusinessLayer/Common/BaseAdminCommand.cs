using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Common
{
    public abstract class BaseAdminCommand: IAdminCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumAdminActions _action;
        protected IAdminService _admin;
        protected IAdminsDbProcess _adminDb;

        public void InitializeCommand(EnumAdminActions action, IContext ctx)
        {
            InitializeCommand(action,ctx, null);
        }

        public void InitializeCommand(EnumAdminActions action, IContext ctx, object param)
        {
            _action = action;
            _context = ctx;
            _param = param;
            _adminDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            _admin = DmsResolver.Current.Get<IAdminService>();
        }

        public InternalDocument Document => null;
        public IContext Context => _context;
        public object Parameters => _param;

        public abstract bool CanBeDisplayed(int positionId);

        public abstract bool CanExecute();

        public abstract object Execute();

        public virtual EnumAdminActions CommandType => _action;
    }
}