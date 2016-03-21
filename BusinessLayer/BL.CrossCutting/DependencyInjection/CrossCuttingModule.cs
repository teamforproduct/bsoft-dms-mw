using BL.CrossCutting.Helpers;
using Ninject.Modules;

namespace BL.CrossCutting.DependencyInjection
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConnectionHelper>().To<ConnectionHelper>().InSingletonScope();
        }
    }
}