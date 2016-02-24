﻿using System;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore;
using BL.Logic.DictionaryCore.DocumentType;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.AdditionalCommands;
using BL.Logic.DocumentCore.Commands;
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

            Bind<IDocumentFiltersService>().To<DocumentFiltersService>().InSingletonScope();
            Bind<IDocumentSendListService>().To<DocumentSendListService>().InSingletonScope();
            Bind<IDocumentTagService>().To<DocumentTagService>().InSingletonScope();

            Bind<ICommandService>().To<CommandService>().InSingletonScope();
            LoadDocumentCommands();
            LoadDocumentAdditionCommands();
            LoadDictionaryCommands();
        }
        private void LoadDictionaryCommands()
        {
            Bind<IDictionaryCommand>().To<AddDictionaryDocumentTypeCommand>();
            Bind<IDictionaryCommand>().To<ModifyDictionaryDocumentTypeCommand>();
        }

        private void LoadDocumentCommands()
        {
            Bind<IDocumentCommand>().To<AddDocumentCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentCommand>();
            Bind<IDocumentCommand>().To<CopyDocumentCommand>();

            Bind<IDocumentCommand>().To<ControlChangeDocumentCommand>();
            Bind<IDocumentCommand>().To<ControlOnDocumentCommand>();
            Bind<IDocumentCommand>().To<ControlOffDocumentCommand>();

            Bind<IDocumentCommand>().To<AddFavouriteDocumentCommand>();
            Bind<IDocumentCommand>().To<DeleteFavouriteDocumentCommand>();

            Bind<IDocumentCommand>().To<StartWorkDocumentCommand>();
            Bind<IDocumentCommand>().To<FinishWorkDocumentCommand>();

            Bind<IDocumentCommand>().To<AddNoteDocumentCommand>();
            Bind<IDocumentCommand>().To<SendMessageDocumentCommand>();
            Bind<IDocumentCommand>().To<ChangeExecutorDocumentCommand>();
            Bind<IDocumentCommand>().To<RegisterDocumentCommand>();

        }

        private void LoadDocumentAdditionCommands()
        {
            Bind<IDocumentAdditionCommand>().To<AddDocumentLinkCommand>();

        }

    }
}