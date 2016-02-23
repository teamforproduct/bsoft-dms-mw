using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentsDbProcess
    {
        void AddDocument(IContext ctx, InternalDocument document);
        void UpdateDocument(IContext ctx, InternalDocument document);
        void DeleteDocument(IContext context, int id);

        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FrontDocument GetDocument(IContext ctx, int documentId);

        InternalDocument GetInternalDocument(IContext ctx, int documentId);


        InternalDocument RegisterDocumentPrepare(IContext ctx, RegisterDocument model);
        void SetNextDocumentRegistrationNumber(IContext ctx, InternalDocument document);
        bool VerifyDocumentRegistrationNumber(IContext ctx, InternalDocument document);

        InternalDocument CopyDocumentPrepare(IContext ctx, int documentId);
        InternalDocument AddDocumentByTemplateDocumentPrepare(IContext context, int templateDocumentId);
        InternalDocument DeleteDocumentPrepare(IContext context, int documentId);
        InternalDocument ModifyDocumentPrepare(IContext context, int id);

        InternalDocument ChangeExecutorDocumentPrepare(IContext ctx, ChangeExecutor model);
        void ChangeExecutorDocument(IContext ctx, InternalDocument document);

        InternalDocument PlanDocumentPrepare(IContext context, int documentId);

        void LaunchPlanDocument(IContext ctx, int documentId);

        void StopPlanDocument(IContext ctx, int documentId);
    }
}