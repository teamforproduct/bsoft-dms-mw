using System;
using System.Collections.Generic;
using BL.Model.FullTextSerach;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Linq;
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
        private const string FIELD_DOC_ID = "DocumentId";
        private const string FIELD_OBJECT_TYPE = "ObjectType";
        private const string FIELD_OBJECT_ID = "ObjectId";
        private const string FIELD_BODY = "postBody";
        IndexWriter _writer;


        public FullTextIndexWorker(string serverKey, string storePath)
        {
            _serverKey = serverKey;
            _storePath = storePath;
            _directory = FSDirectory.Open(_storePath);
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
            _indexReader = IndexReader.Open(_directory, true); // only searching, so read-only=true
        }

        public string ServerKey => _serverKey;

        public void DeleteItem(FullTextIndexIem item)
        {
            if (_writer == null) return;
            var qryVal = new[] { item.DocumentId.ToString(), ((int)item.ItemType).ToString(), item.ObjectId.ToString() };
            var fld = new[] { FIELD_DOC_ID, FIELD_OBJECT_TYPE, FIELD_OBJECT_ID };
            var flags = new[] { Occur.MUST, Occur.MUST, Occur.MUST };
            var query = MultiFieldQueryParser.Parse(Version.LUCENE_30, qryVal, fld, flags, _analyzer);
            _writer.DeleteDocuments(query);
        }

        public void AddNewItem(FullTextIndexIem item)
        {
            if (_writer == null) return;
            var doc = new Document();
            doc.Add(new Field(FIELD_DOC_ID, item.DocumentId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(FIELD_OBJECT_TYPE, ((int)item.ItemType).ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(FIELD_OBJECT_ID, item.ObjectId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(FIELD_BODY, item.ObjectText, Field.Store.NO, Field.Index.ANALYZED));
            _writer.AddDocument(doc);
        }

        public void UpdateItem(FullTextIndexIem item)
        {
            if (_writer == null) return;
            DeleteItem(item);
            AddNewItem(item);

            //TODO probably _writer.UpdateDocument can be used here, but how to build Term for multiple field
            //var qryVal = new string[] {documentId.ToString(), ((int) objType).ToString(), objId.ToString()};
            //var fld = new string[] {FIELD_DOC_ID, FIELD_OBJECT_TYPE, FIELD_OBJECT_ID};
            //var flags = new Occur[] {Occur.MUST, Occur.MUST, Occur.MUST};
            //var query = MultiFieldQueryParser.Parse(Version.LUCENE_30, qryVal, fld, flags, _analyzer);
            //var qryRes = _searcher.Search(query, null, 100);
            //var doc = qryRes.ScoreDocs.FirstOrDefault();
            //if (doc != null)
            //{
            //    var rdoc = _searcher.Doc(doc.Doc);
            //    var term = new Term();
            //    _writer.UpdateDocument(term, rdoc,_analyzer);
            //}

        }

        public void StartUpdate()
        {
            _writer = new IndexWriter(_directory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public void CommitChanges()
        {
            _writer.Optimize();
            //Close the writer
            _writer.Commit();
            _writer.Dispose();
            _writer = null;
            _searcher.Dispose();
            _indexReader.Dispose();
            _indexReader = IndexReader.Open(_directory, true);
            _searcher = new IndexSearcher(_indexReader);
        }

        public IEnumerable<FullTextSearchResult> Search(string text)
        {
            //QueryParser parser = new QueryParser(Version.LUCENE_30, "postBody", analyzer);
            var parser = new QueryParser(Version.LUCENE_30, FIELD_BODY, _analyzer);

            //StreamReader strimReader = new StreamReader(queries, Encoding.Default);
            //StreamReader queryReader = new StreamReader(strimReader.BaseStream, strimReader.CurrentEncoding); ;
            Query query = parser.Parse(text);
            var qryRes = _searcher.Search(query, 100);
            var searchResult = new List<FullTextSearchResult>();
            foreach (var doc in qryRes.ScoreDocs.OrderByDescending(x=>x.Score))
            {
                try
                {
                    var rdoc = _searcher.Doc(doc.Doc);
                    var sr = new FullTextSearchResult
                    {
                        DocumentId = Convert.ToInt32(rdoc.Get(FIELD_DOC_ID)),
                        ObjectType = (EnumSearchObjectType) Convert.ToInt32(rdoc.Get(FIELD_OBJECT_TYPE)),
                        ObjectId = Convert.ToInt32(rdoc.Get(FIELD_OBJECT_ID))
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

        public IEnumerable<FullTextSearchResult> Search(string text, EnumSearchObjectType objectType, int documentId)
        {
            return null;
        }

        public void Dispose()
        {
            
        }
    }
}