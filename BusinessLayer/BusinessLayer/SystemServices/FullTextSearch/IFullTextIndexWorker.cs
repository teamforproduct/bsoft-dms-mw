﻿using System;
using System.Collections.Generic;
using BL.Model.FullTextSearch;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public interface IFullTextIndexWorker: IDisposable
    {
        string ServerKey { get; }

        void StartUpdate();
        void CommitChanges();

        void AddNewItem(FullTextIndexIem item);
        void DeleteItem(FullTextIndexIem item);
        void UpdateItem(FullTextIndexIem item);

        IEnumerable<FullTextSearchResult> Search(string text);
        IEnumerable<FullTextSearchResult> Search(string text, EnumSearchObjectType objectType, int documentId);

        void ReindexDatabase(IEnumerable<FullTextIndexIem> items);
    }
}