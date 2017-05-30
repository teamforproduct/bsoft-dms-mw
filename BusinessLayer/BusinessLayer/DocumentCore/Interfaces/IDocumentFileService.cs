using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFileService
    {
        IEnumerable<FrontDocumentFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null);
        FrontDocumentFile GetUserFile(IContext ctx, int id);
        FrontDocumentFile GetUserFilePdf(IContext ctx, int id);
        FrontDocumentFile GetUserFilePreview(IContext ctx, int id);
        void DeleteDocumentFileFinal(IContext ctx);
    }
}