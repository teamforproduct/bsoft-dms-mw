using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;

namespace BL.Database.Documents
{
    public class DocumentSendListsDbProcess : IDocumentSendListsDbProcess
    {

        public FrontDocumentRestrictedSendList GetRestrictedSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentRestrictedSendList(dbContext, ctx, new FilterDocumentRestrictedSendList { Id = new List<int> { id } }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendLists(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentRestrictedSendList(dbContext, ctx, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });
            }
        }

        public IEnumerable<FrontDocumentSendList> GetSendLists(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentSendList(dbContext, ctx, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });
            }
        }

        public FrontDocumentSendList GetSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentSendList(dbContext, ctx, new FilterDocumentSendList { Id = new List<int> { id } }).FirstOrDefault();
            }
        }
  
    }
}