using System;
using System.Collections.Generic;
using BL.Model.FullTextSearch;

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

        IEnumerable<FullTextSearchResult> SearchItems(string text, int clientId, FullTextSearchFilter filter);

        void DeleteAllDocuments(int clientId);
    }
}