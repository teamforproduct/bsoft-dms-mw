using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentFileDbProcess
    {
        IEnumerable<FrontFilterDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<FrontFilterDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber);
        FrontFilterDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber);
        FrontFilterDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        FrontFilterDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber);

        int AddNewFileOrVersion(IContext ctx, FrontFilterDocumentAttachedFile docFile);
        void UpdateFileOrVersion(IContext ctx, FrontFilterDocumentAttachedFile docFile);
        void DeleteAttachedFile(IContext ctx, FrontFilterDocumentAttachedFile docFile);
        int GetNextFileOrderNumber(IContext ctx, int documentId);
        int GetFileNextVersion(IContext ctx, int documentId, int fileOrder);
    }
}