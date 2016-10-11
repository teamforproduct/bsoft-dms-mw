using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using System.Transactions;

namespace BL.Database.Documents
{
    public class DocumentFiltersDbProcess : IDocumentFiltersDbProcess
    {
        public DocumentFiltersDbProcess()
        {
        }
        #region DocumentSavedFilters

        public IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx))
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.DocumentSavedFiltersSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

                //TODO: Uncomment to get the filters on the positions
                //var positionId = dbContext.Context.CurrentPositionId;
                //qry = qry.Where(x => x.PositionId == positionId);

                var res = qry.Select(x => new FrontDocumentSavedFilter
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    Name = x.Name,
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
            using (var dbContext = new DmsContext(ctx))
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {

                var savFilter =
                    dbContext.DocumentSavedFiltersSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(x => x.Id == savedFilterId)
                        .Select(x => new FrontDocumentSavedFilter
                        {
                            Id = x.Id,
                            PositionId = x.PositionId,
                            Name = x.Name,
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