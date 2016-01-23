using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.CrossCutting.Logging;
using Ninject.Modules;

namespace BL.CrossCutting.DependencyInjection
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();

            // TODO: remove in future and use Context separately for each user
            Bind<IContext>().To<DefaultContext>().InSingletonScope();
        }
    }
}