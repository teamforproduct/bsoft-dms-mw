using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.Common
{
    public abstract class BaseAdminCommand: IAdminCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumAdminActions _action;
        protected IAdminService _adminService;
        protected IAdminsDbProcess _adminDb;
        protected ISystemDbProcess _systemDb;
        protected IDictionaryService _dictService;
        protected IDictionariesDbProcess _dictDb;
        protected ILogger _logger;

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
            _adminService = DmsResolver.Current.Get<IAdminService>();
            _dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            _dictService= DmsResolver.Current.Get<IDictionaryService>();
            _systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        public InternalDocument Document => null;
        public IContext Context => _context;
        public object Parameters => _param;

        public abstract bool CanBeDisplayed(int positionId);

        public abstract bool CanExecute();

        public abstract object Execute();

        public virtual EnumAdminActions CommandType => _action;

        public bool TypeModelIs<T>() => _param is T;

        public T GetModel<T>()
        {
            if (!(TypeModelIs<T>()))
            {
                throw new WrongParameterTypeError();
            }
            return (T)_param;
        }
    }
}