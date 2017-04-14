﻿using System.Collections.Generic;
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
                var res = qry.Select(y => new FrontDocumentRestrictedSendList
                {
                    Id = y.Id,
                    DocumentId = y.DocumentId,
                    PositionId = y.PositionId,
                    PositionName = y.Position.Name,
                    PositionExecutorAgentName = y.Position.ExecutorAgent.Name + (y.Position.ExecutorType.Suffix != null ? " (" + y.Position.ExecutorType.Suffix + ")" : null),
                    AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                    AccessLevelName = y.AccessLevel.Name,
                    DepartmentName = y.Position.Department.Name,
                }).FirstOrDefault();
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
                var res = qry.Select(y => new FrontDocumentRestrictedSendList
                {
                    Id = y.Id,
                    DocumentId = y.DocumentId,
                    PositionId = y.PositionId,
                    PositionName = y.Position.Name,
                    PositionExecutorAgentName = y.Position.ExecutorAgent.Name + (y.Position.ExecutorType.Suffix != null ? " (" + y.Position.ExecutorType.Suffix + ")" : null),
                    AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                    AccessLevelName = y.AccessLevel.Name,
                    DepartmentName = y.Position.Department.Name,
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
                        x.Position.Department.FullPath + " " + x.Position.Department.Name,
                    },
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        private void SetAccessGroups(IContext context, List<FrontDocumentSendList> items)
        {
            var dbContext = context.DbContext as DmsContext;
            var qryAcc = dbContext.DocumentSendListAccessGroupsSet.AsQueryable();
            var ids = items.Select(x => x.Id).ToList();
            var filterContains = PredicateBuilder.New<DocumentSendListAccessGroups>(false);
            filterContains = ids.Aggregate(filterContains,
                (current, value) => current.Or(e => e.SendListId == value).Expand());
            qryAcc = qryAcc.Where(filterContains);
            var accGroups = qryAcc.GroupBy(x => x.SendListId).Select(x => new
            {
                EventId = x.Key,
                AccessGroups = x.Select(y => new FrontDocumentSendListAccessGroup
                {
                    AccessType = (EnumEventAccessTypes)y.AccessTypeId,
                    AccessGroupType = (EnumEventAccessGroupTypes)y.AccessGroupTypeId,
                    Name = y.Agent.Name ?? y.Company.Agent.Name ?? y.Department.Name ?? y.Position.Name ?? y.StandartSendList.Name,
                }).ToList(),
            }).ToList();
            items.ForEach(x => x.AccessGroups = accGroups.Where(y => y.EventId == x.Id).Select(y => y.AccessGroups).FirstOrDefault());
        }


        public IEnumerable<FrontDocumentSendList> GetSendLists(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentSendList(context, new FilterDocumentSendList { DocumentId = new List<int> { documentId } }).ToList();
                SetAccessGroups(context, res);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentSendList GetSendList(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentSendList(ctx, new FilterDocumentSendList { Id = new List<int> { id } }).FirstOrDefault();
                SetAccessGroups(ctx, new List<FrontDocumentSendList> { res });
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
                var linkId = dbContext.DocumentsSet.Where(y => y.Id == model.DocumentId)
                    .Where(y => y.Accesses.Any(z => z.PositionId == model.CurrentPositionId && z.IsInWork))
                    .Select(y => y.LinkId).FirstOrDefault();
                var qry = dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId)
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
                var docs = dbContext.DocumentsSet.Where(y => y.LinkId == linkId && y.Id != model.DocumentId)
                    .Where(x => x.Accesses.Any(y => y.PositionId == model.CurrentPositionId))
                     .Select(y => new FrontDocument
                     {
                         Id = y.Id,
                         DocumentDirectionName = y.TemplateDocument.DocumentDirection.Name,
                         DocumentTypeName = y.TemplateDocument.DocumentType.Name,

                         RegistrationNumber = y.RegistrationNumber,
                         RegistrationNumberPrefix = y.RegistrationNumberPrefix,
                         RegistrationNumberSuffix = y.RegistrationNumberSuffix,

                         DocumentDate = y.RegistrationDate ?? y.CreateDate,
                         IsRegistered = y.IsRegistered,
                         Description = y.Description,
                     }).ToList();
                docs.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));
                res.Documents = docs;
                var filterPositionsContains = PredicateBuilder.New<DictionaryPositions>(false);
                filterPositionsContains = model.Positions.Aggregate(filterPositionsContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());
                res.Positions = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId).Where(filterPositionsContains)
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