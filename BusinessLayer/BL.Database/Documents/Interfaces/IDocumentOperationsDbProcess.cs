using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.SystemCore;
using BL.Model.SystemCore.InternalModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentOperationsDbProcess
    {
        void AddDocumentLink(IContext context, InternalDocument model);

        IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext ctx, List<int> positionIds);

        void AddDocumentWaits(IContext ctx, InternalDocument documentWaits);

        void AddDocumentEvents(IContext ctx, InternalDocument document);
        FrontDocumentEvent GetDocumentEvent(IContext ctx, int eventId);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter, UIPaging paging);
        InternalDocument MarkDocumentEventsAsReadPrepare(IContext ctx, int documentId);
        void MarkDocumentEventAsRead(IContext ctx, IEnumerable<InternalDocumentEvent> eventList);

        InternalDocument ChangeIsInWorkAccessPrepare(IContext context, int documentId);
        InternalDocument ChangeIsFavouriteAccessPrepare(IContext context, int documentId);
        InternalDocument ControlOffDocumentPrepare(IContext context, int eventId);
        IEnumerable<InternalDocumentAccess> GetDocumentAccesses(IContext ctx, int documentId);
        InternalDocument LaunchDocumentSendListItemPrepare(IContext context, int model);
        InternalDocument ControlChangeDocumentPrepare(IContext context, int eventId);
        InternalDocument ControlTargetChangeDocumentPrepare(IContext context, int eventId);
        void ChangeIsInWorkAccess(IContext ctx, InternalDocument access);
        InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model);
        void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccess docAccess);

        InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId, string task = null, int id = 0);
        IEnumerable<int> AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendList> model);

        IEnumerable<InternalDocumentRestrictedSendList> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model);

        InternalDocumentRestrictedSendList DeleteDocumentRestrictedSendListPrepare(IContext context, int restSendListId);
        InternalDocument SendForSigningDocumentPrepare(IContext context, InternalDocumentSendList model);
        void CloseDocumentWait(IContext context, InternalDocument document);

        InternalDocument SendForExecutionDocumentPrepare(IContext context, InternalDocumentSendList sendList);

        void DeleteDocumentRestrictedSendList(IContext context, int restSendListId);

        IEnumerable<int> AddDocumentSendList(IContext context, IEnumerable<InternalDocumentSendList> model, IEnumerable<InternalDocumentTask> task = null, IEnumerable<InternalDocumentEvent> paperEvents = null);

        IEnumerable<InternalDocumentSendList> AddByStandartSendListDocumentSendListPrepare(IContext context, ModifyDocumentSendListByStandartSendList model);

        void ModifyDocumentSendList(IContext context, InternalDocumentSendList model, IEnumerable<InternalDocumentTask> task = null, IEnumerable<InternalDocumentEvent> addPaperEvents = null, IEnumerable<int?> delPaperEvents = null);

        InternalDocument DeleteDocumentSendListPrepare(IContext context, int sendListId);

        void DeleteDocumentSendList(IContext context, int sendListId);

        InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId);

        void ChangeDocumentSendListStage(IContext context, IEnumerable<InternalDocumentSendList> model);

        void ModifyDocumentTags(IContext context, InternalDocumentTag model);
        void ChangeDocumentWait(IContext context, InternalDocumentWait wait);
        void ChangeTargetDocumentWait(IContext ctx, InternalDocumentWait wait, InternalDocumentEvent newEvent);
        void SendBySendList(IContext context, InternalDocument document);

        List<int> AddSavedFilter(IContext context, IEnumerable<InternalDocumentSavedFilter> model);
        void ModifySavedFilter(IContext context, InternalDocumentSavedFilter model);
        void DeleteSavedFilter(IContext context, int id);

        DocumentActionsModel GetDocumentActionsModelPrepare(IContext context, int documentId);
        DocumentActionsModel GetDocumentSendListActionsModelPrepare(IContext context, int documentId);
        DocumentActionsModel GetDocumentPaperActionsModelPrepare(IContext context, int documentId);

        void ControlOffSendListPrepare(IContext context, InternalDocument document);
        void ControlOffSubscriptionPrepare(IContext context, InternalDocument document);
        void ControlOffMarkExecutionWaitPrepare(IContext context, InternalDocument document);

        InternalDocument AddNoteDocumentPrepare(IContext context, AddNote model);

        #region DocumentTasks
        InternalDocument ModifyDocumentTaskPrepare(IContext context, ModifyDocumentTasks model);
        IEnumerable<int> AddDocumentTasks(IContext context, IEnumerable<InternalDocumentTask> task);
        InternalDocument DeleteDocumentTaskPrepare(IContext context, int itemId);
        void ModifyDocumentTask(IContext context, InternalDocumentTask task);
        void DeleteDocumentTask(IContext context, int itemId);
        #endregion DocumentTasks

        #region DocumentPapers
        InternalDocument AddDocumentPaperPrepare(IContext context, ModifyDocumentPapers model);
        InternalDocument ChangeDocumentPaperPrepare(IContext context, int paperId);
        InternalDocument EventDocumentPaperPrepare(IContext context, List<int> paperIds, bool isCalcPreLastPaperEvent = false);

        void AddDocumentPapers(IContext context, IEnumerable<InternalDocumentPaper> items);
        void ModifyDocumentPaper(IContext context, InternalDocumentPaper item);
        void DeleteDocumentPaper(IContext context, int itemId);
        void MarkOwnerDocumentPaper(IContext context, InternalDocumentPaper paper);
        void MarkСorruptionDocumentPaper(IContext context, InternalDocumentPaper paper);
        void SendDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> paper);
        void RecieveDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> paper);
        void CancelPlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> paper);
        void PlanDocumentPaperEvent(IContext context, IEnumerable<InternalDocumentPaper> items);
        InternalDocument PlanDocumentPaperEventPrepare(IContext context, List<int> paperIds);
        IEnumerable<InternalDocumentPaper> PlanDocumentPaperFromSendListPrepare(IContext context, int idSendList);
        #endregion DocumentPapers

        #region DocumentPaperLists
        void AddDocumentPaperLists(IContext context, IEnumerable<InternalDocumentPaperList> items);
        InternalDocumentPaperList ChangeDocumentPaperListPrepare(IContext context, int paperId);
        void ModifyDocumentPaperList(IContext context, InternalDocumentPaperList item);
        void DeleteDocumentPaperList(IContext context, int itemId);
        #endregion DocumentPaperLists
    }
}