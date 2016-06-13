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

        IEnumerable<FullTextSearchResult> SearchDocument(string text);
        IEnumerable<FullTextSearchResult> SearchDictionary(string text);
        IEnumerable<FullTextSearchResult> SearchInDocument(string text, int documentId);

        void ReindexDatabase(IEnumerable<FullTextIndexItem> items);
    }
}