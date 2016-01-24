using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Manager;

namespace BL.Database.CoreDb
{
    internal class CoreDb
    {
        protected DmsContext GetUserDmsContext(IContext ctx)
        {
            var manager = DmsResolver.Current.Get<IConnectionManager>();
            return manager.GetDbContext(ctx);
        }
    }
}