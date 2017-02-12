using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.FullTextSearch;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextSearchService: ISystemWorkerService
    {
        void Dispose();
        void ReindexDatabase(IContext ctx);
        IEnumerable<FullTextSearchResult> SearchItems(IContext ctx, string text, FullTextSearchFilter filter);

        IEnumerable<FullTextIndexItem> FullTextIndexPrepareNew(IContext ctx, EnumObjects obj, EnamFilterType filterType, bool isDeepUpdate, bool IsDirectFilter, int idBeg, int idEnd);

    }
}