using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.Common
{
    public abstract class BaseDictionaryCommand: IDictionaryCommand
    {
        protected IContext _context;
        protected object _param;
        private EnumDictionaryActions _action;
        protected IAdminService _admin;
        protected IDictionariesDbProcess _dictDb;

        public void InitializeCommand(EnumDictionaryActions action, IContext ctx)
        {
            InitializeCommand(action,ctx, null);
        }

        public void InitializeCommand(EnumDictionaryActions action, IContext ctx, object param)
        {
            _action = action;
            _context = ctx;
            _param = param;
            _dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            _admin = DmsResolver.Current.Get<IAdminService>();
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