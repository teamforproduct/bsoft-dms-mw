using System.Collections.Generic;
using BL.Model.FullTextSerach;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextIndexWorker : IFullTextIndexWorker
    {
        private readonly string _serverKey;
        private readonly string _storePath;

        public FullTextIndexWorker(string serverKey, string storePath)
        {
            _serverKey = serverKey;
            _storePath = storePath;
        }

        public string ServerKey
        {
            get { return _serverKey; }
        }

        public void DeleteItem()
        {
        }

        public void AddNewItem()
        {
        }

        public void UpdateItem()
        {
        }

        public void CommitChanges()
        {
        }

        public IEnumerable<FullTextSearchResult> Search(string text)
        {
            return null;
        }

        public IEnumerable<FullTextSearchResult> Search(string text, EnumSearchObjectType objectType, int documentId)
        {
            return null;
        }

        public void Dispose()
        {
            
        }
    }
}