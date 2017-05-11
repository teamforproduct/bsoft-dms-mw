﻿using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Linq;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.LuceneVersion;
using BL.Model.SystemCore;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Extensions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Queries;
using Lucene.Net.QueryParsers.Classic;

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
        private const string FIELD_SECURITY = "Security";
        private const string FIELD_FILTERS = "Filters";
        private const string FIELD_DATE_FROM_ID = "DateFrom";
        private const string FIELD_DATE_TO_ID = "DateTo";
        private const string FIELD_FEATURE_ID = "FeatureId";

        private IndexWriter _writer;
        private readonly Directory _directory;
        private readonly Analyzer _analyzer;
        private IndexReader _indexReader;
        private IndexSearcher _searcher;
        private IndexWriterConfig _cfg;

        public FullTextIndexWorker(string serverKey, string storePath)
        {
            _serverKey = serverKey;
            _storePath = storePath;

            var dir = Path.Combine(_storePath, _serverKey.Replace("\\", "").Replace(",", "").Replace(".", "").Replace("/", "_"));
            if (!System.IO.Directory.Exists(dir))
            {
                var di = System.IO.Directory.CreateDirectory(dir);
                Directory directory = FSDirectory.Open(di);
                //Analyzer analyzer = new CaseInsensitiveWhitespaceAnalyzer();
                //IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
                var analyzer = new StandardAnalyzer(Version.LUCENE_48);
                _cfg = new IndexWriterConfig(Version.LUCENE_48, analyzer);
                var writer = new IndexWriter(directory, _cfg);
                writer.Commit();
                writer.Dispose();
            }
            _directory = FSDirectory.Open(new DirectoryInfo(dir));
            _analyzer = new StandardAnalyzer(Version.LUCENE_48);//CaseInsensitiveWhitespaceAnalyzer();
            _indexReader = IndexReader.Open(_directory); // only searching, so read-only=true
            _searcher = new IndexSearcher(_indexReader);
        }

        public string ServerKey => _serverKey;

        public void DeleteItem(FullTextIndexItem item)
        {
            if (_writer == null) return;

            var qry1 = NumericRangeQuery.NewIntRange(FIELD_OBJECT_TYPE, (int)item.ObjectType, (int)item.ObjectType, true, true);
            var qry2 = NumericRangeQuery.NewIntRange(FIELD_OBJECT_ID, item.ObjectId, item.ObjectId, true, true);
            var bQuery = new BooleanQuery();
            bQuery.Add(qry1, BooleanClause.Occur.MUST);
            bQuery.Add(qry2, BooleanClause.Occur.MUST);
            _writer.DeleteDocuments(bQuery);
            if (item.OperationType == EnumOperationType.Delete)
            {
                qry1 = NumericRangeQuery.NewIntRange(FIELD_PARENT_TYPE, (int)item.ObjectType, (int)item.ObjectType, true, true);
                qry2 = NumericRangeQuery.NewIntRange(FIELD_PARENT_ID, item.ObjectId, item.ObjectId, true, true);
                bQuery = new BooleanQuery();
                bQuery.Add(qry1, BooleanClause.Occur.MUST);
                bQuery.Add(qry2, BooleanClause.Occur.MUST);
                _writer.DeleteDocuments(bQuery);
            }
        }

        public void AddNewItem(FullTextIndexItem item)
        {
            if (_writer == null) return;
            var doc = new Document();

            var docIdFld = new Field(FIELD_PARENT_ID, item.ParentObjectId.ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = true });
            //docIdFld.SetIntValue(item.ParentObjectId);
            doc.Add(docIdFld);

            var typeParentFld = new Field(FIELD_PARENT_TYPE, item.ParentObjectType.ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = true });
            //typeParentFld.SetIntValue((int)item.ParentObjectType);
            doc.Add(typeParentFld);

            var typeIdFld = new Field(FIELD_OBJECT_TYPE, item.ObjectType.ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = true });
            //typeIdFld.SetIntValue((int)item.ObjectType);
            doc.Add(typeIdFld);

            var objIdFld = new Field(FIELD_OBJECT_ID, item.ObjectId.ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = true });
            //objIdFld.SetIntValue(item.ObjectId);
            doc.Add(objIdFld);

            var moduleIdFld = new Field(FIELD_MODULE_ID, item.ModuleId.ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = true });
            //moduleIdFld.SetIntValue(item.ModuleId);
            doc.Add(moduleIdFld);

            var featureIdFld = new Field(FIELD_FEATURE_ID, item.FeatureId.ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = true });
            //featureIdFld.SetIntValue(item.FeatureId);
            doc.Add(featureIdFld);

            var clientIdFld = new Field(FIELD_CLIENT_ID, item.ClientId.ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = true });
            //clientIdFld.SetIntValue(item.ClientId);
            doc.Add(clientIdFld);

            var objectText = (item.ObjectText ?? "") + item.ObjectTextAddDateTime.ListToString();
            doc.Add(new Field(FIELD_BODY, objectText, StringField.TYPE_NOT_STORED));

            var securCodes = (item.Access == null || item.Access.All(x => x.Key == 0))
                ? FullTextFilterTypes.NoFilter
                : string.Join(" ", item.Access.Where(x => x.Key != 0).Select(x => x.Key.ToString() + x.Info).ToList());
            doc.Add(new StringField(FIELD_SECURITY, securCodes, Field.Store.YES));

            doc.Add(new StringField(FIELD_FILTERS, item.Filter ?? "", Field.Store.YES));

            var dateFrom = new Field(FIELD_DATE_FROM_ID, (item.DateFrom.HasValue ? (int)item.DateFrom.Value.ToOADate() : 0).ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = false });
            //dateFrom.SetIntValue(item.DateFrom.HasValue ? (int)item.DateFrom.Value.ToOADate() : 0);
            doc.Add(dateFrom);

            var dateTo = new Field(FIELD_DATE_TO_ID, (item.DateTo.HasValue ? (int)item.DateTo.Value.ToOADate() : 0).ToString(), new FieldType { NumericTypeValue = FieldType.NumericType.INT, Tokenized = true, Indexed = true, Stored = false });
            //dateTo.SetIntValue(item.DateTo.HasValue ? (int)item.DateTo.Value.ToOADate() : int.MaxValue);
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
            _writer = new IndexWriter(_directory, _cfg);
        }

        public void CommitChanges()
        {
            //            _writer.Optimize();
            _writer.Commit();
            _writer.Dispose();
            _writer = null;
            _indexReader = IndexReader.Open(_directory);
            _searcher = new IndexSearcher(_indexReader);
        }

        public IEnumerable<FullTextSearchResult> SearchItems(out bool IsNotAll, string text, int clientId, FullTextSearchFilter filter, UIPaging paging = null)
        {
            var searchResult = new List<FullTextSearchResult>();
            var boolQry = new BooleanQuery();
            var boolFilter = new BooleanFilter();

            #region boolQry
            var arr = text.Trim().Split(' ').ToList();
            arr.ForEach(x => x.Trim());
            arr.RemoveAll(string.IsNullOrEmpty);
            var qryString = arr.Aggregate("", (current, elem) => current + (string.IsNullOrEmpty(current) ? $"*{elem}*" : $" AND *{elem}*"));
            var parser = new QueryParser(Version.LUCENE_48, FIELD_BODY, _analyzer) { AllowLeadingWildcard = true };
            var textQry = parser.Parse(qryString);
            boolQry.Add(textQry, BooleanClause.Occur.MUST);

            if (filter.Accesses != null && filter.Accesses.Any())
            {
                var localParser = new QueryParser(Version.LUCENE_48, FIELD_SECURITY, _analyzer);
                var boolLocalQry = new BooleanQuery();
                foreach (var item in filter.Accesses)
                {
                    var localQry = localParser.Parse(item);
                    boolLocalQry.Add(localQry, BooleanClause.Occur.SHOULD);
                }
                boolQry.Add(boolLocalQry, BooleanClause.Occur.MUST);
            }
            if (filter.Filters != null && filter.Filters.Any())
            {
                var localParser = new QueryParser(Version.LUCENE_48, FIELD_FILTERS, _analyzer) { AllowLeadingWildcard = true };
                var boolLocalQry = new BooleanQuery();
                foreach (var item in filter.Filters)
                {
                    var localQry = localParser.Parse(item);
                    boolLocalQry.Add(localQry, BooleanClause.Occur.MUST);
                }
                boolQry.Add(boolLocalQry, BooleanClause.Occur.MUST);
            }

            #endregion boolQry

            #region boolFilter
            var clientFilter = NumericRangeFilter.NewIntRange(FIELD_CLIENT_ID, clientId, clientId, true, true);
            boolFilter.Add(new FilterClause(clientFilter, BooleanClause.Occur.MUST));

            if (filter.ParentObjectId.HasValue)
            {
                var parentIdFilter = NumericRangeFilter.NewIntRange(FIELD_PARENT_ID, filter.ParentObjectId.Value, filter.ParentObjectId.Value, true, true);
                boolFilter.Add(new FilterClause(parentIdFilter, BooleanClause.Occur.MUST));
            }

            if (filter.ParentObjectType.HasValue)
            {
                var parentFilter = NumericRangeFilter.NewIntRange(FIELD_PARENT_TYPE, (int)filter.ParentObjectType.Value, (int)filter.ParentObjectType.Value, true, true);
                boolFilter.Add(new FilterClause(parentFilter, BooleanClause.Occur.MUST));
            }

            if (filter.ObjectType.HasValue)
            {
                var objFilter = NumericRangeFilter.NewIntRange(FIELD_OBJECT_TYPE, (int)filter.ObjectType.Value, (int)filter.ObjectType.Value, true, true);
                boolFilter.Add(new FilterClause(objFilter, BooleanClause.Occur.MUST));
            }

            if (!string.IsNullOrEmpty(filter.Module))
            {
                var moduleFilter = NumericRangeFilter.NewIntRange(FIELD_MODULE_ID, Modules.GetId(filter.Module), Modules.GetId(filter.Module), true, true);
                boolFilter.Add(new FilterClause(moduleFilter, BooleanClause.Occur.MUST));
            }

            if (!string.IsNullOrEmpty(filter.Feature))
            {
                var featureFilter = NumericRangeFilter.NewIntRange(FIELD_FEATURE_ID, Features.GetId(filter.Feature), Features.GetId(filter.Feature), true, true);
                boolFilter.Add(new FilterClause(featureFilter, BooleanClause.Occur.MUST));
            }

            if (filter.IsOnlyActual)
            {
                var currDat = (int)DateTime.Now.ToOADate();
                var fromDat = NumericRangeFilter.NewIntRange(FIELD_DATE_FROM_ID, 0, currDat, true, true);
                //var maxDate = (int)DateTime.Now.AddYears(20).ToOADate();
                var toDat = NumericRangeFilter.NewIntRange(FIELD_DATE_TO_ID, currDat, int.MaxValue, true, true);
                boolFilter.Add(new FilterClause(fromDat, BooleanClause.Occur.MUST));
                boolFilter.Add(new FilterClause(toDat, BooleanClause.Occur.MUST));
            }
            #endregion boolFilter

            var rowLimit = filter?.RowLimit ?? int.MaxValue;

            searchResult = GetQueryResult(out IsNotAll, rowLimit, text, boolQry, boolFilter, paging);

            return searchResult;
        }

        private List<FullTextSearchResult> GetQueryResult(out bool IsNotAll, int rowLimit, string text, BooleanQuery boolQry, BooleanFilter boolFilter, UIPaging paging)
        {

            var sort = new Sort(/*SortField.FIELD_SCORE,*/ new SortField(FIELD_PARENT_ID, SortField.Type_e.INT, true));
            var qryRes = _searcher.Search(boolQry, boolFilter, rowLimit, sort);
            FileLogger.AppendTextToFile($"{DateTime.Now} '{text}' TotalHits: {qryRes.TotalHits} rows SearchInLucena  Query: '{boolQry.ToString()}'", @"C:\TEMPLOGS\fulltext.log");

            if (qryRes.TotalHits >= rowLimit)
                IsNotAll = true;
            else
                IsNotAll = false;

            List<int> lucDocs;
            if (IsNotAll && (paging.IsOnlyCounter ?? false))
                return new List<FullTextSearchResult>();

            if (IsNotAll && paging != null && !(paging.IsOnlyCounter ?? false) && qryRes.ScoreDocs.All(x => x is FieldDoc))
            {
                lucDocs = qryRes.ScoreDocs.Select(x => new { doc = (int)(((FieldDoc)x).Fields[0]), luc = x.Doc })
                    .GroupBy(x => x.doc).OrderByDescending(x => x.Key)
                    .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize)
                    .SelectMany(x => x.Select(y => y.luc)).ToList();
            }
            else //TODO если не будем комбирировать с фильтрами из БД, то документы без счетчиков можно выбирать согласно пейджингу в любом случаее
            {
                lucDocs = qryRes.ScoreDocs.Select(x => x.Doc).ToList();
            }
            var res = lucDocs.AsParallel()
                .Select(doc => new { doc, luc = _searcher.Doc(doc) })
                .Select(rdoc => new FullTextSearchResult
                {
                    ParentId = Convert.ToInt32(rdoc.luc.Get(FIELD_PARENT_ID)),
                    ParentObjectType = (EnumObjects)Convert.ToInt32(rdoc.luc.Get(FIELD_PARENT_TYPE)),
                    ObjectType = (EnumObjects)Convert.ToInt32(rdoc.luc.Get(FIELD_OBJECT_TYPE)),
                    ObjectId = Convert.ToInt32(rdoc.luc.Get(FIELD_OBJECT_ID)),
                    ModuleId = Convert.ToInt32(rdoc.luc.Get(FIELD_MODULE_ID)),
                    FeatureId = Convert.ToInt32(rdoc.luc.Get(FIELD_FEATURE_ID)),
                    Security = rdoc.luc.Get(FIELD_SECURITY),
                    Filters = rdoc.luc.Get(FIELD_FILTERS),
                    //Score = rdoc.doc.Score
                }).ToList();
            FileLogger.AppendTextToFile($"{DateTime.Now} '{text}' FetchRowsFromLucena: {res.Count} rows", @"C:\TEMPLOGS\fulltext.log");
            return res;
        }

        public void DeleteAllDocuments(int clientId)
        {
            var clientQry = NumericRangeQuery.NewIntRange(FIELD_CLIENT_ID, clientId, clientId, true, true);
            _writer.DeleteDocuments(clientQry);
            //_writer.Optimize();
            _writer.Commit();
        }

        public void Dispose()
        {

        }
    }
}