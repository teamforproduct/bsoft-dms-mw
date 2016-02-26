using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFiltersService
    {
        IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx);
        FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
    }
}