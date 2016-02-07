using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentAdditional;

namespace BL.Database.Documents
{
    public interface IDocumentFileDbProcess
    {
        IEnumerable<DocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<DocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber);
        DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber);
        DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        DocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber);

        int AddNewFileOrVersion(IContext ctx, DocumentAttachedFile docFile);
        void UpdateFileOrVersion(IContext ctx, DocumentAttachedFile docFile);
        void DeleteAttachedFile(IContext ctx, DocumentAttachedFile docFile);
        int GetNextFileOrderNumber(IContext ctx, int documentId);
        int GetFileNextVersion(IContext ctx, int documentId, int fileOrder);
    }
}