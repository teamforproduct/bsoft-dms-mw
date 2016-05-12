using BL.CrossCutting.CryptographicWorker;
using BL.CrossCutting.Helpers;
using Ninject.Modules;

namespace BL.CrossCutting.DependencyInjection
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConnectionHelper>().To<ConnectionHelper>().InSingletonScope();

            Bind<ICryptoService>().To<CryptoService>().InSingletonScope();

            Bind<IConvertToDataSet>().To<ConvertToDataSet>().InSingletonScope();
        }
    }
}