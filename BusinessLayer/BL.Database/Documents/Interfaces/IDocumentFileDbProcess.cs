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
        IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null);
        FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber);
        int CheckFileForDocument(IContext ctx, int documentId, string fileName, string fileExt);
        InternalDocument AddDocumentFilePrepare(IContext ctx, int documentId);
        int AddNewFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile);
        void UpdateFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile);
        void UpdateFilePdfView(IContext ctx, InternalDocumentAttachedFile docFile);
        IEnumerable<InternalDocumentAttachedFile> GetOldPdfForAttachedFiles(IContext ctx);
        void RenameFile(IContext ctx, IEnumerable<InternalDocumentAttachedFile> docFiles, IEnumerable<InternalDocumentEvent> docFileEvents);
        InternalDocument ModifyDocumentFilePrepare(IContext ctx, int documentId, int orderNumber, int version);
        InternalDocument RenameDocumentFilePrepare(IContext ctx, int documentId, int orderNumber);
        InternalDocument DeleteDocumentFilePrepare(IContext ctx, FilterDocumentFileIdentity flIdent);
        void DeleteAttachedFile(IContext ctx, InternalDocumentAttachedFile docFile);
        int GetNextFileOrderNumber(IContext ctx, int documentId);
        int GetFileNextVersion(IContext ctx, int documentId, int fileOrder);
    }
}