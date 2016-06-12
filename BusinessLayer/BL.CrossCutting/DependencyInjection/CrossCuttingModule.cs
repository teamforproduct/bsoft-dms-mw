using BL.CrossCutting.CryptographicWorker;
using BL.CrossCutting.Helpers;
using Ninject.Modules;

namespace BL.CrossCutting.DependencyInjection
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICryptoService>().To<CryptoService>().InSingletonScope();

            Bind<IConvertToDataSet>().To<ConvertToDataSet>().InSingletonScope();
        }
    }
}