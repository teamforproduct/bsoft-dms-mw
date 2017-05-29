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
        IEnumerable<FrontDocumentFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null);
        FrontDocumentFile GetDocumentFile(IContext ctx, int id);
        int CheckFileForDocument(IContext ctx, int documentId, string fileName, string fileExt);
        InternalDocument AddDocumentFilePrepare(IContext ctx, int documentId);
        int AddDocumentFile(IContext ctx, InternalDocumentFile docFile);
        void UpdateFileOrVersion(IContext ctx, InternalDocument doc);
        void UpdateFilePdfView(IContext ctx, InternalDocumentFile docFile);
        IEnumerable<InternalDocumentFile> GetOldPdfForAttachedFiles(IContext ctx, int pdfAge);
        void RenameFile(IContext ctx, IEnumerable<InternalDocumentFile> docFiles, IEnumerable<InternalDocumentEvent> docFileEvents);
        InternalDocument ModifyDocumentFilePrepare(IContext ctx, int fileId);
        InternalDocument RenameDocumentFilePrepare(IContext ctx, int documentId, int orderNumber);
        InternalDocument DeleteDocumentFilePrepare(IContext ctx, int id);
        void DeleteAttachedFile(IContext ctx, InternalDocumentFile docFile);
        int GetNextFileOrderNumber(IContext ctx, int documentId);
        int GetFileNextVersion(IContext ctx, int documentId, int fileOrder);
        InternalDocumentFile GetDocumentFileInternal(IContext ctx, int fileId);
    }
}