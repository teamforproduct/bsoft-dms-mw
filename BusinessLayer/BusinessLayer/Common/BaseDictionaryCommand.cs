using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins;
using BL.Database.Dictionaries;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.Common
{
    public abstract class BaseDictionaryCommand : IDictionaryCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumActions _action;
        protected IAdminService _adminService;
        protected AdminsDbProcess _adminDb;
        protected IDictionaryService _dictService;
        protected DictionariesDbProcess _dictDb;
        protected ILogger _logger;

        public void InitializeCommand(EnumActions action, IContext ctx)
        {
            InitializeCommand(action, ctx, null);
        }

        public void InitializeCommand(EnumActions action, IContext ctx, object model)
        {
            _action = action;
            _context = ctx;
            _param = model;
            _adminDb = DmsResolver.Current.Get<AdminsDbProcess>();
            _adminService = DmsResolver.Current.Get<IAdminService>();
            _dictDb = DmsResolver.Current.Get<DictionariesDbProcess>();
            _dictService = DmsResolver.Current.Get<IDictionaryService>();
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        public InternalDocument Document => null;
        public IContext Context => _context;
        public object Parameters => _param;

        public abstract bool CanBeDisplayed(int positionId);

        public abstract bool CanExecute();

        public abstract object Execute();

        public virtual EnumActions CommandType => _action;

        public bool TypeModelIs<T>() => TypeModelIs<T>(_param);

        private bool TypeModelIs<T>(object param) => param is T;

        public T GetModel<T>()
        {
            return GetModel<T>(_param);
        }

        private T GetModel<T>(object param)
        {
            if (!(TypeModelIs<T>()))
            {
                throw new WrongParameterTypeError();
            }
            return (T)param;
        }
    }
}