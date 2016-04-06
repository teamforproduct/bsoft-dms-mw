using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFileService
    {
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterDocumentAttachedFile filter);
        FrontDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent);

        //IEnumerable<FrontDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, FilterDocumentFileIdentity fileIdent);
        //FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent);
        //FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        //FrontDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, FilterDocumentFileIdentity fileIdent);

        //void DeleteDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent);

        //FrontDocumentAttachedFile AddNewVersion(IContext ctx, ModifyDocumentFile model);

    }
}