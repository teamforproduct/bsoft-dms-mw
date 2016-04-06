using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.DocumentCore
{
    public class CommandService : ICommandService
    {
        private readonly IDocumentOperationsDbProcess _operationDb;
        private List<ICommandObserver> _documentObservers;

        public CommandService(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
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

        private static void MenuFormation(IContext ctx, DocumentActionsModel model)
        {
// total list of type for possible actions we could process
            var totalCommandListType = new List<EnumDocumentActions>();

            foreach (var cmd in model.ActionsList.Values)
            {
                var newCmd = cmd.Select(x => x.DocumentAction).Except(totalCommandListType).ToList();
                totalCommandListType.AddRange(newCmd);
            }

            // create full list of possible commands in case to avoid creating duplicate command multiple time
            var totalCommandList = new List<IDocumentCommand>();
            totalCommandListType.ForEach(
                x => totalCommandList.Add(DocumentCommandFactory.GetDocumentCommand(x, ctx, model.Document, null)));
            totalCommandList = totalCommandList.Where(x => x != null).ToList();
                //TODO remove when all command will be implemented

            //for each position check his actions
            foreach (var pos in model.PositionWithActions)
            {
                var actionList = model.ActionsList[pos.Id];
                var resultActions = new List<InternalSystemAction>();
                if (actionList != null)
                {
                    foreach (var act in actionList)
                    {
                        var cmd = totalCommandList.FirstOrDefault(x => x.CommandType == act.DocumentAction);
                        if (cmd != null)
                        {
                            // each command should add to action list of entries, where that action can be executed
                            if (cmd.CanBeDisplayed(pos.Id))
                            {
                                act.ActionRecords = cmd.ActionRecords;
                                resultActions.Add(act);
                            }
                        }
                    }
                    pos.Actions = resultActions;
                }
            }
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId)
        {
            var model = _operationDb.GetDocumentActionsModelPrepare(ctx, documentId);

            MenuFormation(ctx, model);

            return model.PositionWithActions;
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentSendListActions(IContext ctx, int documentId)
        {
            var model = _operationDb.GetDocumentSendListActionsModelPrepare(ctx, documentId);

            MenuFormation(ctx, model);

            return model.PositionWithActions;
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentPaperActions(IContext ctx, int documentId)
        {
            var model = _operationDb.GetDocumentPaperActionsModelPrepare(ctx, documentId);

            MenuFormation(ctx, model);

            return model.PositionWithActions;
        }

    }
}