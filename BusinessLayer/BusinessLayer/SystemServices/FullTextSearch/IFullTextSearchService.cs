using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextSearchService: ISystemWorkerService
    {
        void Dispose();
        void ReindexDatabase(IContext ctx);
        List<FullTextSearchResult> SearchItems(out bool IsNotAll, IContext ctx, string text, FullTextSearchFilter filter, UIPaging paging = null);
        List<int> SearchItemParentId(out bool IsNotAll, IContext ctx, string text, FullTextSearchFilter filter, UIPaging paging = null);
        List<int> SearchItemId(out bool IsNotAll, IContext ctx, string text, FullTextSearchFilter filter, UIPaging paging = null);
    }
}