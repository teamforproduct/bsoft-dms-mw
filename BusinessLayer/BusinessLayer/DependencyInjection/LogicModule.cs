using BL.CrossCutting.Interfaces;
using BL.Logic.Settings;
using Ninject.Modules;

namespace BusinessLayer.DependencyInjection
{
    public class LogicModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISettings>().To<Setting>().InSingletonScope();
        }
    }
}