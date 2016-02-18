using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Database.Documents
{
    public class DocumentTagsDbProcess : IDocumentTagsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentTagsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        #region DocumentTags
        public IEnumerable<BaseDocumentTag> GetTags(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var items = dbContext.DocumentTagsSet
                    .Where(x => x.DocumentId == documentId)
                    .Where(x => !x.Tag.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0))
                    .Select(x => new BaseDocumentTag
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        TagId = x.TagId,
                        Color = x.Tag.Color,
                        Name = x.Tag.Name
                    }).ToList();

                return items;
            }
        }

        public BaseDocumentTag GetTag(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var item = dbContext.DocumentTagsSet
                    .Where(x => x.Id == id)
                    .Where(x => !x.Tag.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0))
                    .Select(x => new BaseDocumentTag
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        TagId = x.TagId,
                        Color = x.Tag.Color,
                        Name = x.Tag.Name
                    }).FirstOrDefault();

                return item;
            }
        }
        #endregion DocumentTags         
    }
}