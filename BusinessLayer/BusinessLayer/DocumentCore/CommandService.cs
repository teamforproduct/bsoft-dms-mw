using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore
{
    public class CommandService : ICommandService
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private readonly IDictionariesDbProcess _dictDb;
        private readonly ISystemDbProcess _sysDb;
        private List<ICommandObserver> _documentObservers;

        public CommandService(IDocumentOperationsDbProcess operationDb, IDictionariesDbProcess dictDb, ISystemDbProcess sysDb)
        {
            _operationDb = operationDb;
            _dictDb = dictDb;
            _sysDb = sysDb;
            _documentObservers = new List<ICommandObserver>();
            var obs = DmsResolver.Current.GetAll<ICommandObserver>();
            _documentObservers.AddRange(obs);
        }


        public object ExecuteCommand(ICommand cmd)
        {
            var docCommand = cmd as IDocumentCommand;

            if (docCommand != null)
            {
                foreach (var obs in _documentObservers.Where(x => x.ObserverType == EnumObserverType.Before))
                {
                    obs.Inform(docCommand.Context, docCommand.Document, docCommand.CommandType, docCommand.Parameters);
                }
            }

            if (cmd.CanExecute())
            {
                return cmd.Execute();
            }

            if (docCommand != null)
            {
                foreach (var obs in _documentObservers.Where(x=>x.ObserverType == EnumObserverType.After))
                {
                    obs.Inform(docCommand.Context, docCommand.Document, docCommand.CommandType, docCommand.Parameters);
                }
            }

            return null;
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId)
        {
            var document = _operationDb.GetDocumentActionsPrepare(ctx, documentId);
            var positions = _dictDb.GetDictionaryPositionsWithActions(ctx, new FilterDictionaryPosition { PositionId = ctx.CurrentPositionsIdList });
            //var systemDb = DmsResolver.Current.Get<ISystemDbProcess>();
            //foreach (var position in positions)
            //{
            //    position.Actions = systemDb.GetSystemActions(ctx, new FilterSystemAction() { Object = EnumObjects.Documents, IsAvailable = true, PositionsIdList = new List<int> { position.Id } });
            //    if (document.IsRegistered || position.Id != document.ExecutorPositionId)
            //    {
            //        position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.ModifyDocument).ToList();
            //        position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.DeleteDocument).ToList();
            //    }
            //    if (document.IsRegistered)
            //    {
            //        position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.RegisterDocument).ToList();
            //        position.Actions = position.Actions.Where(x => x.DocumentAction != EnumDocumentActions.ChangeExecutor).ToList();
            //    }
            //    position.Actions.Where(x => x.DocumentAction == EnumDocumentActions.ControlOff).ToList()
            //        .ForEach(x =>
            //        {
            //            x.ActionRecords = new List<InternalActionRecord>()
            //            {
            //                new InternalActionRecord() {Id = 1, Description = "TEST1"},
            //                new InternalActionRecord() {Id = 2, Description = "TEST2"}
            //            };
            //        });
            //}

            var cmdList = Enum.GetValues(typeof (EnumDocumentActions)).Cast<EnumDocumentActions>()
                .Select(da => DocumentCommandFactory.GetDocumentCommand(da, ctx, document, null))
                .Where(cmd => cmd.CanBeDisplayed()).Cast<ICommand>().ToList();

            return positions;//actions;
        }
    }
}