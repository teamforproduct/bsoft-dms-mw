using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentAdditional;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFileService
    {
        IEnumerable<DocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<DocumentAttachedFile> GetDocumentFileVersions(IContext ctx, DocumentFileIdentity fileIdent);
        DocumentAttachedFile GetDocumentFileVersion(IContext ctx, DocumentFileIdentity fileIdent);
        DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        DocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, DocumentFileIdentity fileIdent);


        void DeleteDocumentFile(IContext ctx, DocumentFileIdentity fileIdent);
        void DeleteDocumentFileVersion(IContext ctx, DocumentFileIdentity fileIdent);
        DocumentAttachedFile GetUserFile(IContext ctx, DocumentFileIdentity fileIdent);
        int AddUserFile(IContext ctx, ModifyDocumentFile model);
        DocumentAttachedFile AddNewVersion(IContext ctx, ModifyDocumentFile model);
        DocumentAttachedFile UpdateCurrentFileVersion(IContext ctx, ModifyDocumentFile model);
    }
}