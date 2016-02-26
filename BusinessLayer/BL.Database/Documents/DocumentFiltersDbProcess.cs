using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

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

        public IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = dbContext.DocumentSavedFiltersSet.AsQueryable();

                //TODO: Uncomment to get the filters on the positions
                //var positionId = dbContext.Context.CurrentPositionId;
                //qry = qry.Where(x => x.PositionId == positionId);

                var res = qry.Select(x => new FrontDocumentSavedFilter
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

        public FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                var savFilter =
                    dbContext.DocumentSavedFiltersSet.Where(x => x.Id == savedFilterId)
                        .Select(x => new FrontDocumentSavedFilter
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
        #endregion DocumentSavedFilters         
    }
}