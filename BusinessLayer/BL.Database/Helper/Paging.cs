using BL.Model.Enums;
using BL.Model.SystemCore;
using System.Data.Entity;
using System.Linq;

namespace BL.Database.Helper
{
    public static class Paging
    {

        public static EnumPagingResult Set<TDbModel>(ref IQueryable<TDbModel> qry, UIPaging paging)
        {
            if (paging == null) return EnumPagingResult.NoPaging;

            // Пагинация.Вернет только количество записей если = true
            // Вернет количество записей и данные если = null
            // Вернет только данные если = false
            // По умолчанию null

            if (paging.IsOnlyCounter ?? true)
            { // IsOnlyCounter in (null, true)
                paging.TotalItemsCount = qry.Count();
            }

            if (paging.IsOnlyCounter ?? false)
            { // IsOnlyCounter in (true)
                return EnumPagingResult.IsOnlyCounter;
            }

            if (paging.IsAll)
            { return EnumPagingResult.IsAll; }
            else
            {
                int skip = paging.PageSize * (paging.CurrentPage - 1);
                int take = paging.PageSize;
                qry = qry.Skip(() => skip).Take(() => take);
                return EnumPagingResult.IsPaging;
            }

        }
    }
}
