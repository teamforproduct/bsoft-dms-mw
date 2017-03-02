using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Helper;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.SystemCore
{
    public static class FTS
    {
        public static IEnumerable<MainFront> Get<MainFront>(IContext context, string module, FullTextSearch searchFilter, IBaseFilter filter, UIPaging paging, UISorting sorting,
            Func<IContext, IBaseFilter, UIPaging, UISorting, IEnumerable<MainFront>> MainFunc,
            Func<IContext, IBaseFilter, UISorting, List<int>> IdsFunc,
            FullTextSearchFilter ftsFilter = null)
        {
            IEnumerable<MainFront> res;

            if (!string.IsNullOrEmpty(searchFilter?.FullTextSearchString))
            {
                if (ftsFilter == null) ftsFilter = new FullTextSearchFilter { Module = module };

                // Получаю список ид из полнотекста
                var ftList = DmsResolver.Current.Get<IFullTextSearchService>().SearchItemParentId(context, searchFilter.FullTextSearchString, ftsFilter);

                // Если полнотекст ничего не нашел...
                if (!ftList.Any()) return new List<MainFront>();

                var list = new List<int>();

                // Если фильтр не задан и результат полнотекста влезает в PageSize 
                // (список ID из базы можно не джойнить, а сортировка примениться в  MainFunc)
                if (filter == null && ftList.Count <= paging.PageSize)
                {
                    list = ftList;
                }
                else
                {
                    // Получаю список ид из таблицы сущности с учетом сортировки
                    var dbList = IdsFunc(context, filter, sorting);

                    // Если в базе ничего не нашлось...
                    if (!dbList.Any()) return new List<MainFront>();

                    // Нахожу пересечение множеств ftList и dbList с сохранением сортировки
                    var sortList = dbList.Select((x, i) => new { Id = x, Index = i }).ToList();

                    list = ftList.Join(sortList, o => o, i => i.Id, (o, i) => i)
                        .OrderBy(x => x.Index).Select(x => x.Id).ToList();

                    // Если после джойна ничего не осталось...
                    if (!list.Any()) return new List<MainFront>();
                }


                // Накладываю параметры пагинации на список
                if (Paging.Set(ref list, paging) == EnumPagingResult.IsOnlyCounter) return new List<MainFront>();

                filter.IDs = list;

                res = MainFunc(context, filter, null, sorting);

                // Если явно не запретили
                if (!searchFilter.IsDontSaveSearchQueryLog && (paging.IsOnlyCounter ?? false) && res.Any())
                {
                    DmsResolver.Current.Get<ILogger>().AddSearchQueryLog(context, module, searchFilter?.FullTextSearchString);
                }
            }
            else
            {
                res = MainFunc(context, filter, paging, sorting);
            }

            return res;
        }

    }
}
