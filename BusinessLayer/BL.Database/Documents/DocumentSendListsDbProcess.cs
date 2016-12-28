using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using System.Transactions;
using BL.CrossCutting.Helpers;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using LinqKit;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.Dictionary;
using BL.Model.DictionaryCore.FrontModel;

namespace BL.Database.Documents
{
    public class DocumentSendListsDbProcess : IDocumentSendListsDbProcess
    {

        public FrontDocumentRestrictedSendList GetRestrictedSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentRestrictedSendList(dbContext, ctx, new FilterDocumentRestrictedSendList { Id = new List<int> { id } }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendLists(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentRestrictedSendList(dbContext, ctx, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDocumentSendList> GetSendLists(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentSendList(dbContext, ctx, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentSendList GetSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentSendList(dbContext, ctx, new FilterDocumentSendList { Id = new List<int> { id } }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public InternalAdditinalLinkedDocumentSendListsPrepare GetAdditinalLinkedDocumentSendListsPrepare(IContext ctx, AdditinalLinkedDocumentSendList model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                InternalAdditinalLinkedDocumentSendListsPrepare res = new InternalAdditinalLinkedDocumentSendListsPrepare();
                res.SendTypeName = dbContext.DictionarySendTypesSet.Where(x => x.Id == (int)EnumSendTypes.SendForInformation).Select(x => x.Name).FirstOrDefault();
                var linkId = dbContext.DocumentsSet.Where(y => y.Id == model.DocumentId)
                    .Where(y => y.Accesses.Any(z => z.PositionId == model.CurrentPositionId && z.IsInWork))
                    .Select(y => y.LinkId).FirstOrDefault();
                var qry = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId != model.DocumentId && x.Document.LinkId == linkId);
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
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
                    .Where(x=>x.Accesses.Any(y=>y.PositionId == model.CurrentPositionId))
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
                docs.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));
                res.Documents = docs;
                var filterPositionsContains = PredicateBuilder.False<DictionaryPositions>();
                filterPositionsContains = model.Positions.Aggregate(filterPositionsContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());
                res.Positions = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId).Where(filterPositionsContains)
                    .Select(x=> new FrontDictionaryPosition
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