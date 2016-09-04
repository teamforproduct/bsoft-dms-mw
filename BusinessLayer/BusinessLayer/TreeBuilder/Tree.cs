using BL.CrossCutting.Extensions;
using BL.Database.Common;
using BL.Model.Common;
using BL.Model.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Logic.TreeBuilder
{
    public static class Tree
    {
        public static List<TreeItem> Get(List<TreeItem> flatList, FilterTree filter)
        {

            bool startWithCondition = (filter.StartWithTreeId ?? string.Empty) != string.Empty;

            var res = GetBranch(flatList, filter, string.Empty, startWithCondition);

            if ((filter.Name ?? string.Empty) != string.Empty)
            {
                var safeList = new List<string>();

                GetSafeList(res, safeList, filter);

                if (safeList.Count > 0)
                {
                    flatList.RemoveAll(r => !safeList.Contains(r.TreeId));
                    res = GetBranch(flatList, filter, string.Empty, startWithCondition);
                }
                else
                {
                    res = null;
                }
            }

            return res;
        }


        private static List<TreeItem> GetBranch(List<TreeItem> flatList, FilterTree filter, string path = "", bool startWithCondition = false)
        {
            var list = new List<TreeItem>();

            if (flatList != null)
            {
                foreach (var item in flatList)
                    if (startWithCondition ? IsStartWithItem(item, filter) : IsNeighbourItem(item, filter))
                    {
                        item.IsUsed = true;
                        item.Path = ((path == string.Empty) ? "" : (path + "/")) + item.TreeId;
                        list.Add(item);
                        item.Childs = GetBranch(flatList, new FilterTree() { StartWithTreeParentId = item.TreeId }, item.Path);
                    }
            }

            return list;
        }

        private static void GetSafeList(List<TreeItem> tree, List<string> safeList, FilterTree filter)
        {
            if (tree != null)
            {
                var arr = CommonFilterUtilites.GetWhereExpressions(filter.Name.ToLower());

                foreach (var item in tree)
                {
                    if (item.Name.ToLower().ContainsArray(arr))
                    {
                        safeList.AddRange(item.Path.Split('/'));
                    }
                    GetSafeList((List<TreeItem>)item.Childs, safeList, filter);
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

    }
}
