using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.FullTextSearch;

namespace BL.Database.SystemDb
{
    public interface IFullTextDbProcess
    {
        List<EnumObjects> ObjectToReindex();
        int GetCurrentMaxCasheId(IContext ctx);
        IEnumerable<FullTextIndexItem> FullTextIndexToUpdate(IContext ctx, int maxIdValue);
        List<int> GetItemsToUpdateCount(IContext ctx, EnumObjects objectType, bool isDeepUpdate);
        IEnumerable<FullTextIndexItem> GetItemsToReindex(IContext ctx, EnumObjects objectType,int? itemCount, int? offset);
        IEnumerable<FullTextIndexItem> FullTextIndexPrepareNew(IContext ctx, EnumObjects objectType, bool isDeepUpdate, bool IsDirectFilter, int? idBeg, int? idEnd);
        void FullTextIndexDeleteProcessed(IContext ctx, IEnumerable<int> processedIds, bool deleteSimilarObject = false);
        void FullTextIndexDeleteCash(IContext ctx, int deleteBis);
    }
}