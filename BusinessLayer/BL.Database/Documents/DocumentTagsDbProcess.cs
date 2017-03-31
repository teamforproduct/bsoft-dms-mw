using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using LinqKit;
using BL.Database.DBModel.Document;
using BL.CrossCutting.Helpers;

namespace BL.Database.Documents
{
    public class DocumentTagsDbProcess : IDocumentTagsDbProcess
    {
        public DocumentTagsDbProcess()
        {
        }

        #region DocumentTags
        public IEnumerable<FrontDocumentTag> GetTags(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DocumentTagsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId).AsQueryable();
                if (!ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.New<DocumentTags>(false);
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Tag.PositionId == value || !e.Tag.PositionId.HasValue).Expand());

                    qry = qry.Where(filterContains);
                }


                var items = qry
                    .Select(x => new FrontDocumentTag
                    {
                        TagId = x.TagId,
                        DocumentId = x.DocumentId,
                        PositionId = x.Tag.PositionId,
                        PositionName = x.Tag.Position.Name,
                        Color = x.Tag.Color,
                        Name = x.Tag.Name,
                        IsSystem = !x.Tag.PositionId.HasValue
                    }).ToList();
                transaction.Complete();
                return items;
            }
        }
        #endregion DocumentTags         
    }
}