using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.CrossCutting.Helpers;

namespace BL.Database.Documents
{
    public class DocumentFiltersDbProcess : IDocumentFiltersDbProcess
    {
        public DocumentFiltersDbProcess()
        {
        }
        #region DocumentSavedFilters

        public IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx, FilterDocumentSavedFilter filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DocumentSavedFiltersSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter != null)
                {
                    if (filter.IsOnlyCurrentUser)
                    {
                        qry = qry.Where(x => x.UserId == ctx.CurrentAgentId || x.IsCommon);
                    }
                }

                var res = qry.Select(x => new FrontDocumentSavedFilter
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Icon = x.Icon,
                    Filter = x.Filter,
                    IsCommon = x.IsCommon,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    UserName = x.User.Agent.Name
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var savFilter =
                    dbContext.DocumentSavedFiltersSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(x => x.Id == savedFilterId)
                        .Select(x => new FrontDocumentSavedFilter
                        {
                            Id = x.Id,
                            UserId = x.UserId,
                            Name = x.Name,
                            Icon = x.Icon,
                            Filter = x.Filter,
                            IsCommon = x.IsCommon,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            UserName = x.User.Agent.Name
                        }).FirstOrDefault();
                transaction.Complete();
                return savFilter;
            }
        }
        #endregion DocumentSavedFilters         
    }
}