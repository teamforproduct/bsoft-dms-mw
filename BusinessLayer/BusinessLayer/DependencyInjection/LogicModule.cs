using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.Logging;
using BL.Logic.Secure;
using BL.Logic.Settings;
using BL.Logic.SystemLogic;
using Ninject.Modules;

namespace BL.Logic.DependencyInjection
{
    public class LogicModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<ISettings>().To<Setting>().InSingletonScope();
            Bind<IDocumentService>().To<DocumentService>().InSingletonScope();
            Bind<ITemplateDocumentService>().To<TemplateDocumentService>().InSingletonScope();
            Bind<IDictionaryService>().To<DictionaryService>().InSingletonScope();
            Bind<IAdminService>().To<AdminService>().InSingletonScope();
            Bind<ISecureService>().To<SecureService>().InSingletonScope();
            Bind<IFileStore>().To<FileStore>().InSingletonScope();
            Bind<IDocumentFileService>().To<DocumentFileService>().InSingletonScope();
        }
    }
}