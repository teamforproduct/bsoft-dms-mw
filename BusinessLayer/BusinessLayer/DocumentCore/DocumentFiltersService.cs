﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.Filters;

namespace BL.Logic.DocumentCore
{
    public class DocumentFiltersService : IDocumentFiltersService
    {
        private readonly IDocumentFiltersDbProcess _documentDb;

        public DocumentFiltersService(IDocumentFiltersDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        #region DocumentSavedFilters

        public IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx, FilterDocumentSavedFilter filter)
        {
            var savedFilters = _documentDb.GetSavedFilters(ctx, filter).ToList();
            var js = new JavaScriptSerializer();
            for (int i = 0, l = savedFilters.Count; i < l; i++)
            {
                try
                {
                    savedFilters[i].Filter = js.DeserializeObject(savedFilters[i].Filter.ToString());
                }
                catch
                {
                }
            }
            return savedFilters;
        }

        public FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            var savedFilter = _documentDb.GetSavedFilter(ctx, savedFilterId);
            var js = new JavaScriptSerializer();
            try
            {
                savedFilter.Filter = js.DeserializeObject(savedFilter.Filter.ToString());
            }
            catch
            {
            }
            return savedFilter;
        }

        #endregion DocumentSavedFilters
    }
}