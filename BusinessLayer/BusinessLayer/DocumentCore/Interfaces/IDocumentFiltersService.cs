using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFiltersService
    {
        int SaveSavedFilter(IContext context, ModifyDocumentSavedFilter savedFilter);
        IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx);
        BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
        void DeleteSavedFilter(IContext context, int savedFilterId);
    }
}