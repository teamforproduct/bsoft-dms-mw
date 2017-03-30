using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Common
{
    public abstract class BaseSystemCommand: ISystemCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumSystemActions _action;
        protected ISystemService _systemService;
        protected IAdminService _adminService;
        protected ISystemDbProcess _systemDb;

        public void InitializeCommand(EnumSystemActions action, IContext ctx)
        {
            InitializeCommand(action,ctx, null);
        }

        public void InitializeCommand(EnumSystemActions action, IContext ctx, object param)
        {
            _action = action;
            _context = ctx;
            _param = param;
            _systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            _systemService = DmsResolver.Current.Get<ISystemService>();
            _adminService = DmsResolver.Current.Get<IAdminService>();
        }

        public InternalDocument Document => null;
        public IContext Context => _context;
        public object Parameters => _param;

        public abstract bool CanBeDisplayed(int positionId);

        public abstract bool CanExecute();

        public abstract object Execute();

        public virtual EnumSystemActions CommandType => _action;
    }
}