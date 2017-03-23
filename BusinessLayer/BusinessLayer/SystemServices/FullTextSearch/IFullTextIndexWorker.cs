using System;
using System.Collections.Generic;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextIndexWorker: IDisposable
    {
        string ServerKey { get; }

        void StartUpdate();
        void CommitChanges();

        void AddNewItem(FullTextIndexItem item);
        void DeleteItem(FullTextIndexItem item);
        void UpdateItem(FullTextIndexItem item);

        IEnumerable<FullTextSearchResult> SearchItems(out bool IsNotAll, string text, int clientId, FullTextSearchFilter filter, UIPaging paging = null);

        void DeleteAllDocuments(int clientId);
    }
}