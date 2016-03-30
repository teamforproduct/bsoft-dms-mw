using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.FullTextSerach;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextSearchService: ISystemWorkerService
    {
        void Dispose();
        void ReindexDatabase(IContext ctx);
        IEnumerable<FullTextSearchResult> Search(IContext ctx, string text);
        IEnumerable<FullTextSearchResult> Search(IContext ctx, string text, EnumSearchObjectType objectType, int documentId);
    }
}