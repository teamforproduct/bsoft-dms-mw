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
using BL.Model.DocumentCore.InternalModel;

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
                    .Where(x => !x.Tag.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0))
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
        public void ModifyDocumentTags(IContext context, InternalDocumentTags model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var dictionaryTags = dbContext.DictionaryTagsSet
                    .Where(x => !x.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.PositionId ?? 0))
                    .Where(x => model.Tags.Contains(x.Id))
                    .Select(x=>x.Id)
                    .ToList();

                var documentTags = dbContext.DocumentTagsSet
                    .Where(x => x.DocumentId == model.DocumentId)
                    .Where(x => !x.Tag.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.Tag.PositionId ?? 0))
                    .Select(x=>x.TagId)
                    .ToList();

                //Удаляем теги которые не присутствуют в списке
                dbContext.DocumentTagsSet
                    .RemoveRange(dbContext.DocumentTagsSet
                        .Where(x=>x.DocumentId==model.DocumentId 
                            && documentTags.Where(y => !dictionaryTags.Contains(y)).Contains(x.TagId)));

                var newDictionaryTags = dictionaryTags
                    .Where(x => !documentTags.Contains(x))
                    .Select(x => new DocumentTags
                    {
                        DocumentId = model.DocumentId,
                        TagId = x,
                        LastChangeUserId = model.LastChangeUserId,
                        LastChangeDate = model.LastChangeDate
                    });

                dbContext.DocumentTagsSet.AddRange(newDictionaryTags);

                dbContext.SaveChanges();
            }
        }
        #endregion DocumentTags         
    }
}