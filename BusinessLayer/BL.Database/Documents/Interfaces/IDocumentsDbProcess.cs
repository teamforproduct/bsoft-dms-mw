using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Reports.FrontModel;
using BL.Model.Enums;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentsDbProcess
    {
        void AddDocument(IContext ctx, InternalDocument document);
        void ModifyDocument(IContext ctx, InternalDocument document, bool isUseInternalSign, bool isUseCertificateSign);
        void DeleteDocument(IContext context, int id);

        void GetCountDocuments(IContext ctx, LicenceInfo licence);
        int GetDocumentIdBySendListId(IContext ctx, int id);
        IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterBase filters, UIPaging paging, EnumGroupCountType? groupCountType = null);
        FrontDocument GetDocument(IContext ctx, int documentId);

        IEnumerable<int> GetLinkedDocumentIds(IContext ctx, int documentId);
        InternalDocument ReportDocumentForDigitalSignaturePrepare(IContext ctx, DigitalSignatureDocumentPdf model);
        FrontReport ReportDocumentForDigitalSignature(IContext ctx, DigitalSignatureDocumentPdf model, bool isUseInternalSign, bool isUseCertificateSign);
        InternalDocument ReportRegistrationCardDocumentPrepare(IContext ctx, int documentId);
        InternalDocument ReportRegistrationCardDocument(IContext ctx, int documentId);

        //InternalDocument ReportTransmissionDocumentPaperEventPrepare(IContext ctx, int documentId)
        List<InternalDocument> ReportRegisterTransmissionDocuments(IContext ctx, int paperListId);

        InternalDocument RegisterDocumentPrepare(IContext ctx, RegisterDocumentBase model);
        InternalDocumnRegistration RegisterModelDocumentPrepare(IContext context, RegisterDocumentBase model);
        void GetNextDocumentRegistrationNumber(IContext ctx, InternalDocument document);
        bool VerifyDocumentRegistrationNumber(IContext ctx, InternalDocument document);

        InternalDocument CopyDocumentPrepare(IContext ctx, int documentId);
        InternalDocument AddDocumentPrepare(IContext context, int templateDocumentId);
        InternalDocument ModifyDocumentPrepare(IContext context, ModifyDocument model);
        InternalDocument DeleteDocumentPrepare(IContext context, int documentId);

        InternalDocument ChangeExecutorDocumentPrepare(IContext ctx, ChangeExecutor model);
        InternalDocument ChangePositionDocumentPrepare(IContext ctx, ChangePosition model);
        void ChangeExecutorDocument(IContext ctx, InternalDocument document);
        void ChangePositionDocument(IContext ctx, ChangePosition model, InternalDocument document);
        void RegisterDocument(IContext context, InternalDocument document);

        InternalDocument ChangeIsLaunchPlanDocumentPrepare(IContext context, int documentId);

        void ChangeIsLaunchPlanDocument(IContext ctx, InternalDocument document);
        //InternalDocument GetBlankInternalDocumentById(IContext context, int documentId);

        #region DocumentPapers

        IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext ctx, FilterDocumentPaper filter, UIPaging paging);

        FrontDocumentPaper GetDocumentPaper(IContext ctx, int id);
        #endregion DocumentPapers   

        #region DocumentPaperLists

        IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext ctx, FilterDocumentPaperList filter, UIPaging paging);

        FrontDocumentPaperList GetDocumentPaperList(IContext ctx, int id);
        #endregion DocumentPaperLists   

        #region DocumentAccesses

        IEnumerable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, FilterDocumentAccess filters, UIPaging paging);

        #endregion DocumentAccesses

        IEnumerable<InternalDocumentEvent> GetEventsNatively(IContext ctx, FilterDocumentEventNatively filter);

        bool ExistsEventsNatively(IContext ctx, FilterDocumentEventNatively filter);
    }
}