using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentsDbProcess
    {
        void AddDocument(IContext ctx, InternalDocument document);
        void ModifyDocument(IContext ctx, InternalDocument document);
        void DeleteDocument(IContext context, int id);

        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter);

        Model.DocumentCore.ReportModel.ReportDocument GetDocumentByReport(IContext ctx, int documentId);

        InternalDocument RegisterDocumentPrepare(IContext ctx, RegisterDocument model);
        void GetNextDocumentRegistrationNumber(IContext ctx, InternalDocument document);
        bool VerifyDocumentRegistrationNumber(IContext ctx, InternalDocument document);

        InternalDocument CopyDocumentPrepare(IContext ctx, int documentId);
        InternalDocument AddDocumentPrepare(IContext context, int templateDocumentId);
        InternalDocument ModifyDocumentPrepare(IContext context, ModifyDocument model);
        InternalDocument DeleteDocumentPrepare(IContext context, int documentId);

        InternalDocument ChangeExecutorDocumentPrepare(IContext ctx, ChangeExecutor model);
        void ChangeExecutorDocument(IContext ctx, InternalDocument document);
        void RegisterDocument(IContext context, InternalDocument document);

        InternalDocument ChangeIsLaunchPlanDocumentPrepare(IContext context, int documentId);

        void ChangeIsLaunchPlanDocument(IContext ctx, InternalDocument document);
        InternalDocument GetBlankInternalDocumentById(IContext context, int documentId);

        #region DocumentPapers

        IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext ctx, FilterDocumentPaper filter);

        FrontDocumentPaper GetDocumentPaper(IContext ctx, int id);
        #endregion DocumentPapers   

        #region DocumentPaperLists

        IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext ctx, FilterDocumentPaperList filter);

        FrontDocumentPaperList GetDocumentPaperList(IContext ctx, int id);
        #endregion DocumentPaperLists   

    }
}