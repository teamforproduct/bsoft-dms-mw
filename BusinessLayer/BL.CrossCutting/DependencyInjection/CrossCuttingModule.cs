using BL.CrossCutting.Helpers;
using Ninject.Modules;

namespace BL.CrossCutting.DependencyInjection
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConnectionStringHelper>().To<ConnectionStringHelper>().InSingletonScope();
        }
    }
}