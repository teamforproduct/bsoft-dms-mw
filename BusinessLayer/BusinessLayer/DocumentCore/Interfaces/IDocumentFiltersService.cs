using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFiltersService
    {
        int SaveSavedFilter(IContext context, ModifyDocumentSavedFilter savedFilter);
        IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx);
        FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
        void DeleteSavedFilter(IContext context, int savedFilterId);
    }
}