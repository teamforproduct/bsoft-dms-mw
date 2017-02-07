using BL.Logic.AdminCore.Interfaces;
using DMS_WebAPI.Utilities;
using Ninject.Modules;

namespace DMS_WebAPI.DependencyInjection
{
    public class ApiModel : NinjectModule
    {
        public override void Load()
        {
            Bind<UserContexts>().ToSelf().InSingletonScope();

            Bind<WebAPIDbProcess>().ToSelf().InSingletonScope();
            Bind<WebAPIService>().ToSelf().InSingletonScope();

            Bind<UserContextsWorkerService>().ToSelf().InSingletonScope();
            Bind<LicencesWorkerService>().ToSelf().InSingletonScope();
            //Bind<Languages>().ToSelf().InSingletonScope();
            Bind<ILanguages>().To <Languages>().InSingletonScope();
        }
    }
}