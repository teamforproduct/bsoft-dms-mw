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
using BL.Model.SystemCore;
using BL.Model.Exception;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextIndexWorker : IFullTextIndexWorker
    {
        private readonly string _serverKey;
        private readonly string _storePath;

        private const string FIELD_PARENT_ID = "ParentId";
        private const string FIELD_PARENT_TYPE = "ParentType";
        private const string FIELD_OBJECT_TYPE = "ObjectType";
        private const string FIELD_OBJECT_ID = "ObjectId";
        private const string FIELD_BODY = "postBody";
        private const string FIELD_CLIENT_ID = "ClientId";
        private const string FIELD_MODULE_ID = "ModuleId";
        private const string FIELD_SECURITY_ID = "SecurityCodes";
        private const string FIELD_DATE_FROM_ID = "DateFrom";
        private const string FIELD_DATE_TO_ID = "DateTo";
        private const string FIELD_FEATURE_ID = "FeatureId";
        private const string NO_RULES_VALUE = "N";
        private const int MAX_DOCUMENT_COUNT_RETURN = 100000;// int.MaxValue;

        private IndexWriter _writer;
        private readonly Directory _directory;
        private readonly Analyzer _analyzer;
        private IndexReader _indexReader;
        private IndexSearcher _searcher;


        public FullTextIndexWorker(string serverKey, string storePath)
        {
            _serverKey = serverKey;
            _storePath = storePath;

            var dir = Path.Combine(_storePath, _serverKey.Replace("\\", "").Replace(",", "").Replace(".", "").Replace("/", "_"));
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
            var qry1 = NumericRangeQuery.NewIntRange(FIELD_OBJECT_TYPE, (int)item.ObjectType, (int)item.ObjectType, true, true);
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
            docIdFld.SetIntValue(item.ParentObjectId);
            doc.Add(docIdFld);

            var typeParentFld = new NumericField(FIELD_PARENT_TYPE, Field.Store.YES, true);
            typeParentFld.SetIntValue((int)item.ParentObjectType);
            doc.Add(typeParentFld);

            var typeIdFld = new NumericField(FIELD_OBJECT_TYPE, Field.Store.YES, true);
            typeIdFld.SetIntValue((int)item.ObjectType);
            doc.Add(typeIdFld);

            var objIdFld = new NumericField(FIELD_OBJECT_ID, Field.Store.YES, true);
            objIdFld.SetIntValue(item.ObjectId);
            doc.Add(objIdFld);


            var moduleId = new NumericField(FIELD_MODULE_ID, Field.Store.YES, true);
            moduleId.SetIntValue(item.ModuleId);
            doc.Add(moduleId);

            var featureId = new NumericField(FIELD_FEATURE_ID, Field.Store.YES, true);
            featureId.SetIntValue(item.FeatureId);
            doc.Add(featureId);

            doc.Add(new Field(FIELD_BODY, item.ObjectText ?? "", Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field(FIELD_CLIENT_ID, item.ClientId.ToString(), Field.Store.NO, Field.Index.ANALYZED));

            var securCodes = (item.Access == null || item.Access.All(x => x == 0)) ? NO_RULES_VALUE : "v" + string.Join("v", item.Access.Where(x => x != 0).ToList()) + "v";
            doc.Add(new Field(FIELD_SECURITY_ID, securCodes, Field.Store.NO, Field.Index.ANALYZED));

            var dateFrom = new NumericField(FIELD_DATE_FROM_ID, Field.Store.NO, true);
            dateFrom.SetIntValue(item.DateFrom.HasValue ? (int)item.DateFrom.Value.ToOADate() : 0);
            doc.Add(dateFrom);

            var dateTo = new NumericField(FIELD_DATE_TO_ID, Field.Store.NO, true);
            dateTo.SetIntValue(item.DateTo.HasValue ? (int)item.DateTo.Value.ToOADate() : 0);
            doc.Add(dateTo);

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

        public IEnumerable<FullTextSearchResult> SearchItems2(string text, int clientId, FullTextSearchFilter filter)
        {
            var searchResult = new List<FullTextSearchResult>();

            text = text.Trim();

            var arr = text.Split(' ').ToList();
            arr.ForEach(x => x.Trim());

            arr.RemoveAll(string.IsNullOrEmpty);

            var qryString = arr.Aggregate("", (current, elem) => current + (string.IsNullOrEmpty(current) ? $"*{elem}*" : $" AND *{elem}*"));

            var parser = new QueryParser(Version.LUCENE_30, FIELD_BODY, _analyzer) { AllowLeadingWildcard = true };
            var textQry = parser.Parse(qryString);
            var clientQry = new TermQuery(new Term(FIELD_CLIENT_ID, clientId.ToString()));
            var boolQry = new BooleanQuery();
            boolQry.Add(textQry, Occur.MUST);
            boolQry.Add(clientQry, Occur.MUST);

            if (filter.ParentObjectType.HasValue)
            {
                var parentQry = NumericRangeQuery.NewIntRange(FIELD_PARENT_TYPE, (int)filter.ParentObjectType.Value, (int)filter.ParentObjectType.Value, true, true);
                boolQry.Add(parentQry, Occur.MUST);
            }

            if (filter.ObjectType.HasValue)
            {
                var objQry = NumericRangeQuery.NewIntRange(FIELD_OBJECT_TYPE, (int)filter.ObjectType.Value, (int)filter.ObjectType.Value, true, true);
                boolQry.Add(objQry, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(filter.Module))
            {
                var moduleQry = NumericRangeQuery.NewIntRange(FIELD_MODULE_ID, Modules.GetId(filter.Module), Modules.GetId(filter.Module), true, true);
                boolQry.Add(moduleQry, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(filter.Feature))
            {
                var featureQry = NumericRangeQuery.NewIntRange(FIELD_FEATURE_ID, Features.GetId(filter.Feature), Features.GetId(filter.Feature), true, true);
                boolQry.Add(featureQry, Occur.MUST);
            }

            if (filter.IsOnlyActual)
            {
                var currDat = (int)DateTime.Now.ToOADate();
                var fromDat = NumericRangeQuery.NewIntRange(FIELD_DATE_FROM_ID, 0, currDat, false, true);
                var maxDate = (int)DateTime.Now.AddYears(20).ToOADate();
                var toDat = NumericRangeQuery.NewIntRange(FIELD_DATE_FROM_ID, currDat, maxDate, true, true);
                boolQry.Add(fromDat, Occur.MUST);
                boolQry.Add(toDat, Occur.MUST);
            }

            if (filter.Accesses != null && filter.Accesses.Any())
            {
                var scrParser = new QueryParser(Version.LUCENE_30, FIELD_SECURITY_ID, _analyzer) { AllowLeadingWildcard = true };
                var scrEmpty = scrParser.Parse(NO_RULES_VALUE);
                var boolScr = new BooleanQuery();
                boolScr.Add(scrEmpty, Occur.SHOULD);
                foreach (var access in filter.Accesses)
                {
                    var scrRule = scrParser.Parse($"*v{access}v*");
                    boolScr.Add(scrRule, Occur.SHOULD);
                }
                boolQry.Add(boolScr, Occur.MUST);
            }
            searchResult = GetQueryResult(text, boolQry);

            return searchResult;
        }

        public IEnumerable<FullTextSearchResult> SearchItems(string text, int clientId, FullTextSearchFilter filter)
        {
            var searchResult = new List<FullTextSearchResult>();

            text = text.Trim();

            var arr = text.Split(' ').ToList();
            arr.ForEach(x => x.Trim());

            arr.RemoveAll(string.IsNullOrEmpty);

            var qryString = arr.Aggregate("", (current, elem) => current + (string.IsNullOrEmpty(current) ? $"*{elem}*" : $" AND *{elem}*"));

            var parser = new QueryParser(Version.LUCENE_30, FIELD_BODY, _analyzer) {AllowLeadingWildcard = true};
            var textQry = parser.Parse(qryString);
            var clientQry = new TermQuery(new Term(FIELD_CLIENT_ID, clientId.ToString()));

            NumericRangeQuery<int> parentQry = null;
            if (filter.ParentObjectType.HasValue)
            {
                parentQry = NumericRangeQuery.NewIntRange(FIELD_PARENT_TYPE, (int)filter.ParentObjectType.Value, (int)filter.ParentObjectType.Value, true, true);
            }

            NumericRangeQuery<int> objQry = null;
            if (filter.ObjectType.HasValue)
            {
                objQry = NumericRangeQuery.NewIntRange(FIELD_OBJECT_TYPE, (int)filter.ObjectType.Value, (int)filter.ObjectType.Value, true, true);
            }

            NumericRangeQuery<int> moduleQry = null;
            if (!string.IsNullOrEmpty(filter.Module))
            {
                moduleQry = NumericRangeQuery.NewIntRange(FIELD_MODULE_ID, Modules.GetId(filter.Module),  Modules.GetId(filter.Module), true, true);
            }

            NumericRangeQuery<int> featureQry = null;
            if (!string.IsNullOrEmpty(filter.Feature))
            {
                featureQry = NumericRangeQuery.NewIntRange(FIELD_FEATURE_ID, Features.GetId(filter.Feature), Features.GetId(filter.Feature), true, true);
            }

            NumericRangeQuery<int> fromDat = null;
            NumericRangeQuery<int> toDat = null;
            if (filter.IsOnlyActual)
            {
                var currDat = (int)DateTime.Now.ToOADate();
                fromDat = NumericRangeQuery.NewIntRange(FIELD_DATE_FROM_ID, 0, currDat, false, true);
                

                var maxDate = (int)DateTime.Now.AddYears(20).ToOADate();
                toDat = NumericRangeQuery.NewIntRange(FIELD_DATE_FROM_ID, currDat, maxDate, true, true);
                
            }

            if (filter.Accesses != null && filter.Accesses.Any())
            {
                var scrParser = new QueryParser(Version.LUCENE_30, FIELD_SECURITY_ID, _analyzer) { AllowLeadingWildcard = true };
                var scrEmpty = scrParser.Parse(NO_RULES_VALUE);
                var boolQry = new BooleanQuery();
                boolQry.Add(textQry, Occur.MUST);
                boolQry.Add(clientQry, Occur.MUST);
                boolQry.Add(scrEmpty, Occur.MUST);
                if (parentQry != null) boolQry.Add(parentQry, Occur.MUST);
                if (objQry != null) boolQry.Add(objQry, Occur.MUST);
                if (moduleQry != null) boolQry.Add(moduleQry, Occur.MUST);
                if (featureQry != null) boolQry.Add(featureQry, Occur.MUST);
                if (fromDat != null) boolQry.Add(fromDat, Occur.MUST);
                if (toDat != null) boolQry.Add(toDat, Occur.MUST);
                searchResult.AddRange(GetQueryResult(text,boolQry));

                foreach (var access in filter.Accesses)
                {
                    var scrQry = scrParser.Parse($"*v{access}v*");
                    boolQry = new BooleanQuery();
                    boolQry.Add(textQry, Occur.MUST);
                    boolQry.Add(clientQry, Occur.MUST);
                    boolQry.Add(scrQry, Occur.MUST);
                    if (parentQry != null) boolQry.Add(parentQry, Occur.MUST);
                    if (objQry != null) boolQry.Add(objQry, Occur.MUST);
                    if (moduleQry != null) boolQry.Add(moduleQry, Occur.MUST);
                    if (featureQry != null) boolQry.Add(featureQry, Occur.MUST);
                    if (fromDat != null) boolQry.Add(fromDat, Occur.MUST);
                    if (toDat != null) boolQry.Add(toDat, Occur.MUST);
                    searchResult.AddRange(GetQueryResult(text,boolQry));
                }
                return searchResult;
            }
            else
            {
                var boolQry = new BooleanQuery();
                boolQry.Add(textQry, Occur.MUST);
                boolQry.Add(clientQry, Occur.MUST);

                if (parentQry != null) boolQry.Add(parentQry, Occur.MUST);
                if (objQry != null) boolQry.Add(objQry, Occur.MUST);
                if (moduleQry != null) boolQry.Add(moduleQry, Occur.MUST);
                if (featureQry != null) boolQry.Add(featureQry, Occur.MUST);
                if (fromDat != null) boolQry.Add(fromDat, Occur.MUST);
                if (toDat != null) boolQry.Add(toDat, Occur.MUST);
                searchResult.AddRange(GetQueryResult(text,boolQry));
                return searchResult;
            }
        }

        private List<FullTextSearchResult> GetQueryResult(string text, BooleanQuery boolQry)
        {
            var sort = new Sort(SortField.FIELD_SCORE, new SortField(FIELD_PARENT_ID, SortField.INT, true));

            var qryRes = _searcher.Search(boolQry, null, MAX_DOCUMENT_COUNT_RETURN, sort);
           //var qryRes = _searcher.Search(boolQry, null, MAX_DOCUMENT_COUNT_RETURN);
            if (qryRes.TotalHits >= MAX_DOCUMENT_COUNT_RETURN)
                throw new SystemFullTextTooManyResults(text);

            return qryRes.ScoreDocs.Where(x => x.Score > 1).OrderByDescending(x => x.Score).AsParallel()
                .Select(doc => new {doc, luc = _searcher.Doc(doc.Doc)})
                .Select(rdoc => new FullTextSearchResult
                {
                    ParentId = Convert.ToInt32(rdoc.luc.Get(FIELD_PARENT_ID)),
                    ParentObjectType = (EnumObjects) Convert.ToInt32(rdoc.luc.Get(FIELD_PARENT_TYPE)),
                    ObjectType = (EnumObjects) Convert.ToInt32(rdoc.luc.Get(FIELD_OBJECT_TYPE)),
                    ObjectId = Convert.ToInt32(rdoc.luc.Get(FIELD_OBJECT_ID)),
                    ModuleId = Convert.ToInt32(rdoc.luc.Get(FIELD_MODULE_ID)),
                    FeatureId = Convert.ToInt32(rdoc.luc.Get(FIELD_FEATURE_ID)),
                    Score = rdoc.doc.Score
                }).ToList();

        }

        public void DeleteAllDocuments(int clientId)
        {
            var clientQry = new TermQuery(new Term(FIELD_CLIENT_ID, clientId.ToString()));

            _writer.DeleteDocuments(clientQry);
            _writer.Optimize();
            _writer.Commit();
        }

        public void Dispose()
        {

        }
    }
}