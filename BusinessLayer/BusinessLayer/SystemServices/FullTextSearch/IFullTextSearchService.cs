using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.Enums;
using BL.Model.FullTextSearch;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextSearchService: ISystemWorkerService
    {
        void Dispose();
        void ReindexDatabase(IContext ctx);
        IEnumerable<FullTextSearchResult> SearchDocument(IContext ctx, string text);
        IEnumerable<FullTextSearchResult> SearchDictionary(IContext ctx, string text);
        IEnumerable<FullTextSearchResult> SearchInDocument(IContext ctx, string text, EnumObjects objectType, int documentId);
    }
}