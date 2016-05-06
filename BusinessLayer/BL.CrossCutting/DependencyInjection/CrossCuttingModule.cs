using BL.CrossCutting.Helpers;
using BL.CrossCutting.Helpers.Crypto;
using Ninject.Modules;

namespace BL.CrossCutting.DependencyInjection
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConnectionHelper>().To<ConnectionHelper>().InSingletonScope();

            Bind<ICryptoService>().To<CryptoService>().InSingletonScope();
        }
    }
}