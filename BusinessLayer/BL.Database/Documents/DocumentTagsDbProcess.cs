using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;

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
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.DocumentTagsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId).AsQueryable();
                if (!ctx.IsAdmin)
                {
                    qry = qry.Where(x => !x.Tag.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0));
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

                return items;
            }
        }
        #endregion DocumentTags         
    }
}