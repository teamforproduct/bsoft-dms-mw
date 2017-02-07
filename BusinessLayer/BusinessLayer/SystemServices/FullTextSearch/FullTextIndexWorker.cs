using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Linq;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextIndexWorker : IFullTextIndexWorker
    {
        private readonly string _serverKey;
        private readonly string _storePath;
        private readonly Directory _directory;
        private Analyzer _analyzer;
        private IndexReader _indexReader;
        private IndexSearcher _searcher;
        private const string FIELD_PARENT_ID = "ParentId";
        private const string FIELD_PARENT_TYPE = "ParentType";
        private const string FIELD_OBJECT_TYPE = "ObjectType";
        private const string FIELD_OBJECT_ID = "ObjectId";
        private const string FIELD_BODY = "postBody";
        private const string FIELD_CLIENT_ID = "ClientId";
        private const int MAX_DOCUMENT_COUNT_RETURN = 100000;
        IndexWriter _writer;

        public FullTextIndexWorker(string serverKey, string storePath)
        {
            _serverKey = serverKey;
            _storePath = storePath;

            var dir = Path.Combine(_storePath,_serverKey.Replace("\\", "").Replace(",", "").Replace(".", "").Replace("/", "_"));
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
                Directory directory = FSDirectory.Open(dir);
                Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
                IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
                writer.Commit();
                writer.Dispose();
            }
            _directory = FSDirectory.Open(dir);
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
            _indexReader = IndexReader.Open(_directory, true); // only searching, so read-only=true
            _searcher = new IndexSearcher(_indexReader);
        }

        public string ServerKey => _serverKey;

        public void DeleteItem(FullTextIndexItem item)
        {
            if (_writer == null) return;
            var qry1 = NumericRangeQuery.NewIntRange(FIELD_OBJECT_TYPE, (int)item.ItemType, (int)item.ItemType, true, true);
            var qry2 = NumericRangeQuery.NewIntRange(FIELD_OBJECT_ID, item.ObjectId, item.ObjectId, true, true); 
            var bQuery = new BooleanQuery();
            bQuery.Add(qry1, Occur.MUST);
            bQuery.Add(qry2, Occur.MUST);

            _writer.DeleteDocuments(bQuery);
        }

        public void AddNewItem(FullTextIndexItem item)
        {
            if (_writer == null) return;
            var doc = new Document();
            var docIdFld = new NumericField(FIELD_PARENT_ID, Field.Store.YES, true);
            docIdFld.SetIntValue(item.ParentId);
            doc.Add(docIdFld);

            var typeParentFld = new NumericField(FIELD_PARENT_TYPE, Field.Store.YES, true);
            typeParentFld.SetIntValue((int)item.ParentItemType);
            doc.Add(typeParentFld);

            var typeIdFld = new NumericField(FIELD_OBJECT_TYPE, Field.Store.YES, true);
            typeIdFld.SetIntValue((int)item.ItemType);
            doc.Add(typeIdFld);

            var objIdFld = new NumericField(FIELD_OBJECT_ID, Field.Store.YES, true);
            objIdFld.SetIntValue(item.ObjectId);
            doc.Add(objIdFld);

            doc.Add(new Field(FIELD_BODY, item.ObjectText??"", Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field(FIELD_CLIENT_ID, item.ClientId.ToString(), Field.Store.NO, Field.Index.ANALYZED));

            _writer.AddDocument(doc);
        }

        public void UpdateItem(FullTextIndexItem item)
        {
            if (_writer == null) return;
            DeleteItem(item);
            AddNewItem(item);
        }

        public void StartUpdate()
        {
            _writer = new IndexWriter(_directory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public void CommitChanges()
        {
            _writer.Optimize();
            _writer.Commit();
            _writer.Dispose();
            _writer = null;
            _indexReader = IndexReader.Open(_directory, true);
            _searcher = new IndexSearcher(_indexReader);
        }

        public IEnumerable<FullTextSearchResult> SearchItems(string text, int clientId, FullTextSearchFilter filter)
        {
            text = text.Trim();

            var arr = text.Split(' ').ToList();
            arr.ForEach(x => x.Trim());

            arr.RemoveAll(string.IsNullOrEmpty);

            var qryString = arr.Aggregate("", (current, elem) => current + (string.IsNullOrEmpty(current) ? $"*{elem}*" : $" AND *{elem}*"));

            var parser = new QueryParser(Version.LUCENE_30, FIELD_BODY, _analyzer);
            parser.AllowLeadingWildcard = true;
            var textQry = parser.Parse(qryString);
            var clientQry = new TermQuery(new Term(FIELD_CLIENT_ID, clientId.ToString()));
            var boolQry = new BooleanQuery();
            boolQry.Add(textQry, Occur.MUST);
            boolQry.Add(clientQry, Occur.MUST);
            if (filter.ParentObjectType.HasValue)
            {
                var parentQry = new TermQuery(new Term(FIELD_PARENT_TYPE, ((int)filter.ParentObjectType).ToString()));
                boolQry.Add(parentQry, Occur.MUST);
            }
            if (filter.ObjectType.HasValue)
            {
                var objQry = new TermQuery(new Term(FIELD_OBJECT_TYPE, ((int)filter.ObjectType).ToString()));
                boolQry.Add(objQry, Occur.MUST);
            }

            var qryRes = _searcher.Search(boolQry, MAX_DOCUMENT_COUNT_RETURN);

            var searchResult = new List<FullTextSearchResult>();

            foreach (var doc in qryRes.ScoreDocs.Where(x => x.Score > 1).OrderByDescending(x => x.Score))
            {
                try
                {
                    var rdoc = _searcher.Doc(doc.Doc);
                    var sr = new FullTextSearchResult
                    {
                        ParentId = Convert.ToInt32(rdoc.Get(FIELD_PARENT_ID)),
                        ParentObjectType = (EnumObjects)Convert.ToInt32(rdoc.Get(FIELD_PARENT_TYPE)),
                        ObjectType = (EnumObjects) Convert.ToInt32(rdoc.Get(FIELD_OBJECT_TYPE)),
                        ObjectId = Convert.ToInt32(rdoc.Get(FIELD_OBJECT_ID)),
                        Score = doc.Score
                    };
                    searchResult.Add(sr);
                }
                catch
                {
                    // ignored
                }
            }
            return searchResult;
        }

        public void DeleteAllDocuments()
        {
            _writer.DeleteAll();
        }

        public void Dispose()
        {
            
        }
    }
}