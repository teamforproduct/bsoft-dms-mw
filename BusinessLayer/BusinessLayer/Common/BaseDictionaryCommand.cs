using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Common
{
    public abstract class BaseDictionaryCommand: IDictionaryCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumDictionaryActions _action;
        protected IAdminService _adminService;
        protected IAdminsDbProcess _adminDb;
        protected IDictionaryService _dictService;
        protected IDictionariesDbProcess _dictDb;
        protected ILogger _logger;

        public void InitializeCommand(EnumDictionaryActions action, IContext ctx)
        {
            InitializeCommand(action,ctx, null);
        }

        public void InitializeCommand(EnumDictionaryActions action, IContext ctx, object param)
        {
            _action = action;
            _context = ctx;
            _param = param;
            _adminDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            _adminService = DmsResolver.Current.Get<IAdminService>();
            _dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            _dictService = DmsResolver.Current.Get<IDictionaryService>();
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        public InternalDocument Document => null;
        public IContext Context => _context;
        public object Parameters => _param;

        public abstract bool CanBeDisplayed(int positionId);

        public abstract bool CanExecute();

        public abstract object Execute();

        public virtual EnumDictionaryActions CommandType => _action;
    }
}