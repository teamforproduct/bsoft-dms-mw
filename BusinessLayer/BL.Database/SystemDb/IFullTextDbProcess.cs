using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.FullTextSearch;

namespace BL.Database.SystemDb
{
    public interface IFullTextDbProcess
    {
        List<EnumObjects> ObjectToReindex();
        int GetEntityNumbers(IContext ctx, EnumObjects objType);
        int GetCurrentMaxCasheId(IContext ctx);
        IEnumerable<FullTextIndexItem> FullTextIndexToDeletePrepare(IContext ctx);
        List<int> GetItemsToUpdateCount(IContext ctx, EnumObjects objectType, bool isDeepUpdate);

        IEnumerable<FullTextIndexItem> GetItemsToReindex(IContext ctx, EnumObjects objectType, bool isDeepUpdate,int? itemCount, int? offset);
        IEnumerable<FullTextIndexItem> FullTextIndexPrepareNew(IContext ctx, EnumObjects objectType, bool isDeepUpdate, bool IsDirectFilter, int? idBeg, int? idEnd);
        void FullTextIndexDeleteProcessed(IContext ctx, IEnumerable<int> processedIds, bool deleteSimilarObject = false);
        void FullTextIndexDeleteCash(IContext ctx, int deleteBis);
    }
}