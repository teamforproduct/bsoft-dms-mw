using System.Collections.Generic;
using BL.Model.FullTextSerach;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextIndexWorker
    {
        string ServerKey { get; }

        void AddNewItem();
        void CommitChanges();
        void DeleteItem();
        IEnumerable<FullTextSearchResult> Search(string text);
        IEnumerable<FullTextSearchResult> Search(string text, EnumSearchObjectType objectType, int documentId);
        void UpdateItem();
    }
}