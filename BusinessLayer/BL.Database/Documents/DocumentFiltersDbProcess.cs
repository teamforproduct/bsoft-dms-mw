using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using System.Transactions;
using BL.Model.DocumentCore.Filters;

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
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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
                return res;
            }
        }

        public FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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
                return savFilter;
            }
        }
        #endregion DocumentSavedFilters         
    }
}