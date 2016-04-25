using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentFileDbProcess
    {
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterDocumentAttachedFile filter, UIPaging paging = null);
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber);
        FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber);
        FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        FrontDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber);
        int CheckFileForDocument(IContext ctx, string fileName, string fileExt);
        InternalDocument AddDocumentFilePrepare(IContext ctx, int documentId);
        int AddNewFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile);
        void UpdateFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile);
        InternalDocument ModifyDocumentFilePrepare(IContext ctx, int documentId, int orderNumber);
        InternalDocument DeleteDocumentFilePrepare(IContext ctx, FilterDocumentFileIdentity flIdent);
        void DeleteAttachedFile(IContext ctx, InternalDocumentAttachedFile docFile);
        int GetNextFileOrderNumber(IContext ctx, int documentId);
        int GetFileNextVersion(IContext ctx, int documentId, int fileOrder);
    }
}