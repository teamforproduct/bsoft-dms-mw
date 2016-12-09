using BL.Model.SystemCore;
using System.Data.Entity;
using System.Linq;

namespace BL.Database.Helper
{
    public static class Paging
    {

        public static void Set<TDbModel>(ref IQueryable<TDbModel> qry, UIPaging paging)
        {
            if (paging == null) return;

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
                //pss Разобраться зачем это
                //return new List<FrontDictionaryAgent>();
            }

            //paging.TotalItemsCount = qry.Count();

            if (!paging.IsAll)
            {
                int skip = paging.PageSize * (paging.CurrentPage - 1);
                int take = paging.PageSize;
                qry = qry.Skip(() => skip).Take(() => take);
            }

        }
    }
}
