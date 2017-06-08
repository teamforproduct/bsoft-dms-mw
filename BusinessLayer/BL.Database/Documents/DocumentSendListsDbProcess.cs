using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.CrossCutting.Helpers;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using LinqKit;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.Dictionary;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Common;

namespace BL.Database.Documents
{
    public class DocumentSendListsDbProcess : IDocumentSendListsDbProcess
    {

        public FrontDocumentRestrictedSendList GetRestrictedSendList(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentRestrictedSendListQuery(ctx, new FilterDocumentRestrictedSendList { Id = new List<int> { id } });
                var res = qry.Select(x => new FrontDocumentRestrictedSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name,
                    PositionExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null),
                    AccessLevel = (EnumAccessLevels)x.AccessLevelId,
                    AccessLevelName = "##l@AccessLevels:" + ((EnumAccessLevels)x.AccessLevelId).ToString() + "@l##",
                    DepartmentName = x.Position.Department.Name,
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int GetRestrictedSendListsCounter(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentRestrictedSendListQuery(ctx, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });
                var res = qry.Count();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendLists(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentRestrictedSendListQuery(ctx, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });
                var res = qry.Select(x => new FrontDocumentRestrictedSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name,
                    PositionExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null),
                    AccessLevel = (EnumAccessLevels)x.AccessLevelId,
                    AccessLevelName = "##l@AccessLevels:" + ((EnumAccessLevels)x.AccessLevelId).ToString() + "@l##",
                    DepartmentName = x.Position.Department.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetRestrictedSendListsForAutocomplete(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentRestrictedSendListQuery(ctx, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });
                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Position.Id,
                    Name = x.Position.ExecutorAgentId.HasValue ? x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null) : "##l@Message:PositionIsVacant@l##",
                    Details = new List<string>
                    {
                        x.Position.Name,
                        x.Position.Department.Code + " " + x.Position.Department.Name,
                    },
                }).ToList();
                transaction.Complete();
                return res;
            }
        }




        public IEnumerable<FrontDocumentSendList> GetSendLists(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentSendListQuery(context, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });
                var res = qry.Select(x => new FrontDocumentSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Stage = x.Stage,
                    SendType = (EnumSendTypes)x.SendTypeId,
                    SendTypeName = x.SendType.Name,
                    SendTypeCode = x.SendType.Code,
                    StageType = (EnumStageTypes?)x.StageTypeId,
                    StageTypeName = x.StageType.Name,
                    StageTypeCode = x.StageType.Code,
                    SendTypeIsImportant = x.SendType.IsImportant,
                    Task = x.Task.Task,
                    IsAddControl = x.IsAddControl,
                    SelfDescription = x.SelfDescription,
                    SelfDueDate = x.SelfDueDate,
                    SelfDueDay = x.SelfDueDay,
                    SelfAttentionDate = x.SelfAttentionDate,
                    SelfAttentionDay = x.SelfAttentionDay,
                    Description = x.Description,
                    AddDescription = x.AddDescription,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    StartEventId = x.StartEventId,
                    CloseEventId = x.CloseEventId,
                    IsInitial = x.IsInitial,
                    AccessLevel = (EnumAccessLevels)x.AccessLevelId,
                    AccessLevelName = "##l@AccessLevels:" + ((EnumAccessLevels)x.AccessLevelId).ToString() + "@l##",
                    StartEvent = x.StartEvent == null
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = x.StartEvent.Id,
                                            EventType = (EnumEventTypes)x.StartEvent.EventTypeId,
                                            EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.StartEvent.EventTypeId).ToString() + "@l##",
                                            Date = x.StartEvent.Date,
                                            Description = x.StartEvent.Description,
                                            AddDescription = x.StartEvent.AddDescription,
                                            DueDate = x.StartEvent.OnWait.Select(z => z.DueDate).FirstOrDefault(),
                                        },
                    CloseEvent = x.CloseEvent == null || x.StartEventId == x.CloseEventId
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = x.CloseEvent.Id,
                                            EventType = (EnumEventTypes)x.CloseEvent.EventTypeId,
                                            EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.CloseEvent.EventTypeId).ToString() + "@l##",
                                            Date = x.CloseEvent.Date,
                                            Description = x.CloseEvent.Description,
                                            AddDescription = x.CloseEvent.AddDescription,
                                            DueDate = null,
                                        },
                }).ToList();
                CommonQueries.SetAccessGroups(context, res);
                CommonQueries.SetAccessGroups(context, res.Where(x => x.StartEvent != null).Select(x => x.StartEvent).Concat(res.Where(x => x.CloseEvent != null).Select(x => x.CloseEvent)).ToList());
                transaction.Complete();
                return res;
            }
        }
        public int GetSendListsCounter(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentSendListQuery(context, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });
                var res = qry.Count();
                transaction.Complete();
                return res;
            }
        }
        public FrontDocumentSendList GetSendList(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentSendListQuery(ctx, new FilterDocumentSendList { Id = new List<int> { id } });
                var res = qry.Select(x => new FrontDocumentSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Stage = x.Stage,
                    SendType = (EnumSendTypes)x.SendTypeId,
                    SendTypeName = x.SendType.Name,
                    SendTypeCode = x.SendType.Code,
                    StageType = (EnumStageTypes?)x.StageTypeId,
                    StageTypeName = x.StageType.Name,
                    StageTypeCode = x.StageType.Code,
                    SendTypeIsImportant = x.SendType.IsImportant,
                    Task = x.Task.Task,
                    IsAddControl = x.IsAddControl,
                    SelfDescription = x.SelfDescription,
                    SelfDueDate = x.SelfDueDate,
                    SelfDueDay = x.SelfDueDay,
                    SelfAttentionDate = x.SelfAttentionDate,
                    SelfAttentionDay = x.SelfAttentionDay,
                    Description = x.Description,
                    AddDescription = x.AddDescription,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    StartEventId = x.StartEventId,
                    CloseEventId = x.CloseEventId,
                    IsInitial = x.IsInitial,
                    AccessLevel = (EnumAccessLevels)x.AccessLevelId,
                    AccessLevelName = "##l@AccessLevels:" + ((EnumAccessLevels)x.AccessLevelId).ToString() + "@l##",
                    StartEvent = x.StartEvent == null
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = x.StartEvent.Id,
                                            EventType = (EnumEventTypes)x.StartEvent.EventTypeId,
                                            EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.StartEvent.EventTypeId).ToString() + "@l##",
                                            Date = x.StartEvent.Date,
                                            Description = x.StartEvent.Description,
                                            AddDescription = x.StartEvent.AddDescription,
                                            DueDate = x.StartEvent.OnWait.Select(z => z.DueDate).FirstOrDefault(),
                                        },
                    CloseEvent = x.CloseEvent == null || x.StartEventId == x.CloseEventId
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = x.CloseEvent.Id,
                                            EventType = (EnumEventTypes)x.CloseEvent.EventTypeId,
                                            EventTypeName = "##l@EventTypes:" + ((EnumEventTypes)x.CloseEvent.EventTypeId).ToString() + "@l##",
                                            Date = x.CloseEvent.Date,
                                            Description = x.CloseEvent.Description,
                                            AddDescription = x.CloseEvent.AddDescription,
                                            DueDate = null,
                                        },
                }).FirstOrDefault();
                CommonQueries.SetAccessGroups(ctx, new List<FrontDocumentSendList> { res });
                CommonQueries.SetAccessGroups(ctx, new List<FrontDocumentEvent> { res.StartEvent, res.CloseEvent });
                transaction.Complete();
                return res;
            }
        }

        public InternalAdditinalLinkedDocumentSendListsPrepare GetAdditinalLinkedDocumentSendListsPrepare(IContext ctx, AdditinalLinkedDocumentSendList model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                InternalAdditinalLinkedDocumentSendListsPrepare res = new InternalAdditinalLinkedDocumentSendListsPrepare();
                res.SendTypeName = dbContext.DictionarySendTypesSet.Where(x => x.Id == (int)EnumSendTypes.SendForInformation).Select(x => x.Name).FirstOrDefault();
                var linkId = CommonQueries.GetDocumentQuery(ctx, new FilterDocument { DocumentId = new List<int> { model.DocumentId }, IsInWork = true })
                    .Select(y => y.LinkId).FirstOrDefault();
                var qry = dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.Client.Id)
                    .Where(x => x.DocumentId != model.DocumentId && x.Document.LinkId == linkId);
                var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterContains = model.Positions.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                filterContains = filterContains.Or(e => e.PositionId == model.CurrentPositionId).Expand();
                qry = qry.Where(filterContains);
                res.Accesses = qry.Where(filterContains).Select(x => new FrontDocumentAccess
                {
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId
                }
                ).ToList();
                var docs = CommonQueries.GetDocumentQuery(ctx, new FilterDocument { LinkId = new List<int> { linkId.Value }, NotContainsDocumentId = new List<int> { model.DocumentId }, IsInWork = true })
                    .Select(x => new FrontDocument
                    {
                        Id = x.Id,
                        DocumentDirectionName = "##l@DocumentDirections:" + ((EnumDocumentDirections)x.DocumentDirectionId).ToString() + "@l##",
                        DocumentTypeName = x.DocumentType.Name,
                        RegistrationNumber = x.RegistrationNumber,
                        RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                        DocumentDate = x.RegistrationDate ?? x.CreateDate,
                        IsRegistered = x.IsRegistered,
                        Description = x.Description,
                    }).ToList();
                docs.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));
                res.Documents = docs;
                var filterPositionsContains = PredicateBuilder.New<DictionaryPositions>(false);
                filterPositionsContains = model.Positions.Aggregate(filterPositionsContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());
                res.Positions = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.Client.Id).Where(filterPositionsContains)
                    .Select(x => new FrontDictionaryPosition
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ExecutorAgentId = x.ExecutorAgentId,
                        ExecutorAgentName = x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null),
                    }).ToList();

                transaction.Complete();
                return res;
            }
        }

    }
}