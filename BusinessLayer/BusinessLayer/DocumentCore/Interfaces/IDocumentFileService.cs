using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFileService
    {
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null);
        FrontDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent);
        FrontDocumentAttachedFile GetUserFilePdf(IContext ctx, FilterDocumentFileIdentity fileIdent);
        FrontDocumentAttachedFile GetUserFilePreview(IContext ctx, FilterDocumentFileIdentity fileIdent);
    }
}