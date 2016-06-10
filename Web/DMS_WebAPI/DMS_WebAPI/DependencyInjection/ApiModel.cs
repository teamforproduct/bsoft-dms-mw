using DMS_WebAPI.Utilities;
using Ninject.Modules;

namespace DMS_WebAPI.DependencyInjection
{
    public class ApiModel : NinjectModule
    {
        public override void Load()
        {
            Bind<UserContext>().ToSelf().InSingletonScope();
            Bind<UserContextWorkerService>().ToSelf().InSingletonScope();
            Bind<LicencesWorkerService>().ToSelf().InSingletonScope();
            Bind<Languages>().ToSelf().InSingletonScope();
        }
    }
}