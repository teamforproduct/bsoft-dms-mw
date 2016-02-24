using BL.Logic.Helpers;
using Ninject.Modules;

namespace BL.Logic.DependencyInjection
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConnectionStringHelper>().To<ConnectionStringHelper>().InSingletonScope();
        }
    }
}