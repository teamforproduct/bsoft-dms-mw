using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.Filters;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentFiltersDbProcess
    {
        IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx, FilterDocumentSavedFilter filter);
        FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
    }
}