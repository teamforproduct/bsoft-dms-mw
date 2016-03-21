﻿using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Context;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;

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
        public IEnumerable<FrontDocumentTag> GetTags(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var items = dbContext.DocumentTagsSet
                    .Where(x => x.DocumentId == documentId)
                    .Where(x => ctx.IsAdmin || !x.Tag.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0))
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