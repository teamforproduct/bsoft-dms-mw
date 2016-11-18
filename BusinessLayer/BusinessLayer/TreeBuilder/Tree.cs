using BL.CrossCutting.Extensions;
using BL.Database.Common;
using BL.Model.Enums;
using BL.Model.Tree;
using System.Collections.Generic;

namespace BL.Logic.TreeBuilder
{
    public static class Tree
    {
        public static List<TreeItem> Get(List<TreeItem> flatList, FilterTree filter)
        {

            bool startWithCondition = (filter?.StartWithTreeId ?? string.Empty) != string.Empty;

            bool notStartWithCondition = (filter?.WithoutTreeId ?? string.Empty) != string.Empty;

            int level = -1;

            var res = GetBranch(flatList, filter, ref level, ref notStartWithCondition, string.Empty, startWithCondition);

            if (filter?.RemoveEmptyBranchesByObject?.Count() > 0)
            {
                var safeList = new List<string>();

                GetSafeListFunk(res, safeList, x => filter.RemoveEmptyBranchesByObject.Contains((EnumObjects)x.ObjectId));

                if (safeList.Count > 0)
                {
                    flatList.RemoveAll(r => !safeList.Contains(r.TreeId));
                    res = GetBranch(flatList, filter, ref level, ref notStartWithCondition, string.Empty, startWithCondition);
                }

            }

            if ((filter?.Name ?? string.Empty) != string.Empty || (filter?.IsChecked ?? false == true))
            {
                var safeList = new List<string>();

                GetSafeList(res, safeList, filter);

                if (safeList.Count > 0)
                {
                    flatList.RemoveAll(r => !safeList.Contains(r.TreeId));
                    res = GetBranch(flatList, filter, ref level, ref notStartWithCondition, string.Empty, startWithCondition);
                }
                else
                {
                    res = null;
                }
            }

            return res;
        }



        public static List<ITreeItem> GetList(List<TreeItem> tree)
        {
            List<ITreeItem> list = new List<ITreeItem>();

            TreeToList(tree, list);

            return list;
        }


        private static void TreeToList(IEnumerable<ITreeItem> tree, List<ITreeItem> list)
        {
            if (tree != null)
            {
                foreach (var item in tree)
                {
                    list.Add(item);
                    TreeToList(item.Childs, list);
                    item.Childs = null;
                }

            }
        }


        private static List<TreeItem> GetBranch(List<TreeItem> flatList, FilterTree filter, ref int level, ref bool notStartWithCondition, string path = "", bool startWithCondition = false)
        {
            var list = new List<TreeItem>();

            if (flatList != null)
            {
                level++;

                foreach (var item in flatList)
                {
                    if (notStartWithCondition)
                    {
                        if (item.TreeId == filter.WithoutTreeId) continue;
                    }

                    if (startWithCondition ? IsStartWithItem(item, filter) : IsNeighbourItem(item, filter))
                    {
                        item.IsUsed = true;
                        item.Level = level;
                        item.Path = ((path == string.Empty) ? "" : (path + "/")) + item.TreeId;
                        list.Add(item);
                        item.Childs = GetBranch(flatList, new FilterTree() { StartWithTreeParentId = item.TreeId, WithoutTreeId = filter?.WithoutTreeId }, ref level, ref notStartWithCondition, item.Path);
                    }
                }

                level--;
            }

            return list;

        }

        private static void GetSafeList(List<TreeItem> tree, List<string> safeList, FilterTree filter)
        {
            if (tree != null)
            {
                var existsNameFilter = !string.IsNullOrEmpty(filter.Name);
                var existsCheckFilter = filter.IsChecked.HasValue;

                string[] arrName = null;

                if (existsNameFilter)
                { arrName = CommonFilterUtilites.GetWhereExpressions(filter.Name.ToLower()); }

                foreach (var item in tree)
                {
                    var addToSafeList = true;

                    if (existsNameFilter & addToSafeList)
                    {
                        // Поиск присходит по специальному полю для поиска
                        addToSafeList = (item.SearchText.ToLower().ContainsArray(arrName));
                    }

                    if (existsCheckFilter & addToSafeList)
                    {
                        addToSafeList = item.IsChecked ?? false;
                    }

                    if (addToSafeList) safeList.AddRange(item.Path.Split('/'));

                    GetSafeList((List<TreeItem>)item.Childs, safeList, filter);
                }
            }
        }

        private static void GetSafeListFunk(List<TreeItem> tree, List<string> safeList, Func<TreeItem, bool> funk)
        {
            if (tree != null)
            {
                foreach (var item in tree)
                {
                    var addToSafeList = funk(item);

                    if (addToSafeList) safeList.AddRange(item.Path.Split('/'));

                    GetSafeListFunk((List<TreeItem>)item.Childs, safeList, funk);
                }
            }
        }

        private static bool IsNeighbourItem(TreeItem item, FilterTree filter)
        {
            //if (item.IsUsed) return false;
            if (filter == null)
            {
                return (item.TreeParentId == string.Empty);
            }
            else
            {
                return (item.TreeParentId == (filter.StartWithTreeParentId ?? string.Empty));
            }
        }

        private static bool IsStartWithItem(TreeItem item, FilterTree filter)
        {
            //if (item.IsUsed) return false;
            if (filter == null)
            {
                return (item.TreeId == string.Empty);
            }
            else
            {
                return (item.TreeId == (filter.StartWithTreeId ?? string.Empty));
            }
        }





        private static void RemoveEmptyBranches(IEnumerable<ITreeItem> tree)
        {
            foreach (var item in tree)
            {
                if (item.IsList ?? false) continue;

                RemoveEmptyBranches(item.Childs);

                if (!ExistsLists(item.Childs))
                {
                    item.Childs = null;
                }

            }

        }

        private static bool ExistsLists(IEnumerable<ITreeItem> list) => list.Select(x => x.IsList == true).Any();


    }
}
