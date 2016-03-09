using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentOperationsDbProcess
    {
        void AddDocumentLink(IContext context, InternalDocument model);

        IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext ctx, List<int> positionIds);

        void AddDocumentWaits(IContext ctx, InternalDocument documentWaits);

        void AddDocumentEvents(IContext ctx, IEnumerable<InternalDocumentEvent> docEvents);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter);

        InternalDocument ChangeIsInWorkAccessPrepare(IContext context, int documentId);
        InternalDocument ChangeIsFavouriteAccessPrepare(IContext context, int documentId);
        InternalDocument ControlOffDocumentPrepare(IContext context, int eventId);
        IEnumerable<InternalDocumentAccess> GetDocumentAccesses(IContext ctx, int documentId);
        InternalDocument LaunchDocumentSendListItemPrepare(IContext context, int model);
        InternalDocument ControlChangeDocumentPrepare(IContext context, int eventId);
        void ChangeIsInWorkAccess(IContext ctx, InternalDocument access);
        InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model);
        void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccess docAccess);

        InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId);
        void AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendList> model);

        IEnumerable<InternalDocumentRestrictedSendList> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model);

        InternalDocumentRestrictedSendList DeleteDocumentRestrictedSendListPrepare(IContext context, int restSendListId);
        InternalDocument SendForSigningDocumentPrepare(IContext _context, InternalDocumentSendList model);
        void CloseDocumentWait(IContext context, InternalDocument document);

        InternalDocument SendForExecutionDocumentPrepare(IContext context, InternalDocumentSendList sendList);

        void DeleteDocumentRestrictedSendList(IContext context, int restSendListId);

        void AddDocumentSendList(IContext context, IEnumerable<InternalDocumentSendList> model);

        IEnumerable<InternalDocumentSendList> AddByStandartSendListDocumentSendListPrepare(IContext context, ModifyDocumentSendListByStandartSendList model);

        void ModifyDocumentSendList(IContext context, InternalDocumentSendList model);

        InternalDocumentSendList DeleteDocumentSendListPrepare(IContext context, int sendListId);

        void DeleteDocumentSendList(IContext context, int sendListId);

        InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId);

        void ChangeDocumentSendListStage(IContext context, IEnumerable<InternalDocumentSendList> model);

        void ModifyDocumentTags(IContext context, InternalDocumentTag model);
        void ChangeDocumentWait(IContext context, InternalDocumentWait wait);
        void SendBySendList(IContext context, InternalDocument document);

        List<int> AddSavedFilter(IContext context, IEnumerable<InternalDocumentSavedFilter> model);
        void ModifySavedFilter(IContext context, InternalDocumentSavedFilter model);
        void DeleteSavedFilter(IContext context, int id);

        DocumentActionsModel GetDocumentActionsModelPrepare(IContext context, int documentId);
        InternalDocument GetDocumentActionsPrepare(IContext context, int documentId);
        void ControlOffSendListPrepare(IContext context, InternalDocument document);
        void ControlOffSubscriptionPrepare(IContext context, InternalDocument document);
        void ControlOffMarkExecutionWaitPrepare(IContext context, InternalDocument document);
    }
}