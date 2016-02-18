using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFileService
    {
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, DocumentFileIdentity fileIdent);
        FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, DocumentFileIdentity fileIdent);
        FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        FrontDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, DocumentFileIdentity fileIdent);


        void DeleteDocumentFile(IContext ctx, DocumentFileIdentity fileIdent);
        void DeleteDocumentFileVersion(IContext ctx, DocumentFileIdentity fileIdent);
        FrontDocumentAttachedFile GetUserFile(IContext ctx, DocumentFileIdentity fileIdent);
        int AddUserFile(IContext ctx, ModifyDocumentFile model);
        IEnumerable<FrontDocumentAttachedFile> AddUserFile(IContext ctx, ModifyDocumentFiles model);
        FrontDocumentAttachedFile AddNewVersion(IContext ctx, ModifyDocumentFile model);
        FrontDocumentAttachedFile UpdateCurrentFileVersion(IContext ctx, ModifyDocumentFile model);
    }
}