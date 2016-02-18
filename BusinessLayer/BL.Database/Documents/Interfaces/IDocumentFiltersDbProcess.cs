using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentFiltersDbProcess
    {
        void AddSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter);
        void UpdateSavedFilters(IContext ctx, ModifyDocumentSavedFilter savedFilter);
        IEnumerable<FrontDocumentSavedFilter> GetSavedFilters(IContext ctx);
        FrontDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
        void DeleteSavedFilter(IContext ctx, int savedFilterId);
    }
}