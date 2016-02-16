using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents
{
    public class DocumentFiltersDbProcess : IDocumentFiltersDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentFiltersDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }
        #region DocumentSavedFilters

        public void AddSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var savFilter = new DocumentSavedFilters
                {
                    PositionId = ctx.CurrentPositionId,
                    Icon = savedFilter.Icon,
                    Filter = savedFilter.Filter.ToString(),
                    IsCommon = savedFilter.IsCommon,
                    LastChangeUserId = ctx.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };

                dbContext.DocumentSavedFiltersSet.Add(savFilter);
                dbContext.SaveChanges();
                savedFilter.Id = savFilter.Id;
            }
        }

        public void UpdateSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var savFilter = dbContext.DocumentSavedFiltersSet.FirstOrDefault(x => x.Id == savedFilter.Id);
                if (savFilter != null)
                {
                    savFilter.Id = savedFilter.Id;
                    savFilter.PositionId = ctx.CurrentPositionId;
                    savFilter.Icon = savedFilter.Icon;
                    savFilter.Filter = savedFilter.Filter.ToString();
                    savFilter.IsCommon = savedFilter.IsCommon;
                    savFilter.LastChangeUserId = ctx.CurrentAgentId;
                    savFilter.LastChangeDate = DateTime.Now;
                }
                dbContext.SaveChanges();
            }
        }

        public IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = dbContext.DocumentSavedFiltersSet.AsQueryable();

                //TODO: Uncomment to get the filters on the positions
                //var positionId = dbContext.Context.CurrentPositionId;
                //qry = qry.Where(x => x.PositionId == positionId);

                var res = qry.Select(x => new BaseDocumentSavedFilter
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    Icon = x.Icon,
                    Filter = x.Filter,
                    IsCommon = x.IsCommon,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    PositionName = x.Position.Name
                }).ToList();
                return res;
            }
        }

        public BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var savFilter =
                    dbContext.DocumentSavedFiltersSet.Where(x => x.Id == savedFilterId)
                        .Select(x => new BaseDocumentSavedFilter
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                            Icon = x.Icon,
                            Filter = x.Filter,
                            IsCommon = x.IsCommon,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            PositionName = x.Position.Name
                        }).FirstOrDefault();
                return savFilter;
            }
        }
        public void DeleteSavedFilter(IContext ctx, int savedFilterId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var savFilter = dbContext.DocumentSavedFiltersSet.Where(x => x.Id == savedFilterId).FirstOrDefault();
                if (savFilter != null)
                {
                    dbContext.DocumentSavedFiltersSet.Remove(savFilter);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion DocumentSavedFilters         
    }
}