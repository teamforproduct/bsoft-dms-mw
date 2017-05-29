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
                foreach (var obs in _documentObservers.Where(x => x.ObserverType == EnumObserverType.After))
                {
                    obs.Inform(docCommand.Context, docCommand.Document, docCommand.CommandType, docCommand.Parameters);
                }
            }

            return null;
        }

        private static void MenuFormation(IContext ctx, DocumentActionsModel model, bool isOnlyIdActions = false, List<EnumObjects> mаndatoryObject = null)
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
            if (model.PositionWithActions?.Any() ?? false)
                foreach (var pos in model.PositionWithActions)
                {
                    var actionList = model.ActionsList[pos.Id];
                    var resultActions = new List<InternalSystemActionForDocument>();
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
                                    act.ActionRecords = cmd.ActionRecords?.ToList();
                                    if (((mаndatoryObject?.Any() ?? false) && mаndatoryObject.Contains(act.Object)) || !isOnlyIdActions || (act.ActionRecords?.Any() ?? false))
                                    {
                                        resultActions.Add(act);
                                    }
                                }
                            }
                        }
                        pos.Actions = resultActions;
                    }
                }
            model.PositionWithActions.SelectMany(x => x.Actions).Where(x => x.ActionRecords != null).SelectMany(x => x.ActionRecords).Where(x => x != null).ToList()
            .ForEach(x =>
            {
                x.Name = $"Название для {x.EventId}";
                x.Details = new List<string> { "Детали 1", "Детали 2" };
            });
        }
        private static void MenuCategoryGrouping(IContext ctx, DocumentActionsModel model)
        {
            model.PositionWithActions.ForEach(x =>
               x.Categories = x.Actions.GroupBy(y => y.Category).Select(y => new InternalSystemActionCategoryForDocument
               {
                   Category = y.Key,
                   CategoryName = "##l@EnumActionCategories:" + ((EnumActionCategories)y.Key).ToString() + "@l##",
                   Actions = y.ToList()
               }).ToList()
            );
        }
        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int? documentId, int? id = null)
        {
            var model = _operationDb.GetDocumentActionsModelPrepare(ctx, documentId, id);
            MenuFormation(ctx, model, id.HasValue, new List<EnumObjects> { { EnumObjects.DocumentEvents } });
            model.PositionWithActions = model.PositionWithActions?.Where(x => x.Actions != null && x.Actions.Any()).ToList();
            MenuCategoryGrouping(ctx, model);
            return model.PositionWithActions;
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentSendListActions(IContext ctx, int documentId)
        {
            var model = _operationDb.GetDocumentSendListActionsModelPrepare(ctx, documentId);

            MenuFormation(ctx, model);
            MenuCategoryGrouping(ctx, model);
            return model.PositionWithActions;
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentFileActions(IContext ctx, int? documentId, int? id = null)
        {
            var model = _operationDb.GetDocumentFileActionsModelPrepare(ctx, documentId, id);

            MenuFormation(ctx, model, id.HasValue);
            MenuCategoryGrouping(ctx, model);
            return model.PositionWithActions.Where(x => x.Actions != null && x.Actions.Any()).ToList();
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDocumentPaperActions(IContext ctx, int documentId)
        {
            var model = _operationDb.GetDocumentPaperActionsModelPrepare(ctx, documentId);

            MenuFormation(ctx, model);
            MenuCategoryGrouping(ctx, model);
            return model.PositionWithActions;
        }

    }
}