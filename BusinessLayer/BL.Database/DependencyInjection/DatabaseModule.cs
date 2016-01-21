using BL.Database.Factory;
using Ninject.Modules;

namespace BL.Database.DependencyInjection
{
    public class DatabaseModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatabaseFactory>().To<DatabaseFactory>().InSingletonScope();
        }
    }
}