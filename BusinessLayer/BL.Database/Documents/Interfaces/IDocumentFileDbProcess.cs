using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentFileDbProcess
    {
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber);
        FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber);
        FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        FrontDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber);

        int AddNewFileOrVersion(IContext ctx, FrontDocumentAttachedFile docFile);
        void UpdateFileOrVersion(IContext ctx, FrontDocumentAttachedFile docFile);
        void DeleteAttachedFile(IContext ctx, FrontDocumentAttachedFile docFile);
        int GetNextFileOrderNumber(IContext ctx, int documentId);
        int GetFileNextVersion(IContext ctx, int documentId, int fileOrder);
    }
}