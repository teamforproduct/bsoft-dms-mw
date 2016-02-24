using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentOperationsDbProcess
    {
        void AddDocumentLink(IContext context, InternalDocument model);

        IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext ctx, List<int> positionIds);

        InternalDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId);
        void AddDocumentWait(IContext ctx, InternalDocumentWaits documentWait);
        void UpdateDocumentWait(IContext ctx, InternalDocumentWaits documentWait);

        int AddDocumentEvent(IContext ctx, InternalDocumentEvents docEvent);
        void AddDocumentEvents(IContext ctx, IEnumerable<InternalDocumentEvents> docEvents);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter);

        int AddDocumentAccess(IContext ctx, InternalDocumentAccesses access);
        void RemoveDocumentAccess(IContext ctx, int accessId);
        void UpdateDocumentAccess(IContext ctx, InternalDocumentAccesses access);

        InternalDocument ChangeIsInWorkAccessPrepare(IContext context, int documentId);
        InternalDocument ChangeIsFavouriteAccessPrepare(IContext context, int documentId);
        IEnumerable<InternalDocumentAccesses> GetDocumentAccesses(IContext ctx, int documentId);

        void ChangeIsInWorkAccess(IContext ctx, InternalDocument access);
        InternalDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model);
        InternalDocument GetDocumentActionsPrepare(IContext context, int documentId);
        void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccesses docAccess);

        InternalDocument ChangeDocumentSendListPrepare(IContext context, int documentId);
        void AddDocumentRestrictedSendList(IContext context, IEnumerable<InternalDocumentRestrictedSendLists> model);

        IEnumerable<InternalDocumentRestrictedSendLists> AddByStandartSendListDocumentRestrictedSendListPrepare(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model);

        InternalDocumentRestrictedSendLists DeleteDocumentRestrictedSendListPrepare(IContext context, int restSendListId);

        void DeleteDocumentRestrictedSendList(IContext context, int restSendListId);

        void AddDocumentSendList(IContext context, IEnumerable<InternalDocumentSendLists> model);

        IEnumerable<InternalDocumentSendLists> AddByStandartSendListDocumentSendListPrepare(IContext context, ModifyDocumentSendListByStandartSendList model);

        void ModifyDocumentSendList(IContext context, InternalDocumentSendLists model);

        InternalDocumentSendLists DeleteDocumentSendListPrepare(IContext context, int sendListId);

        void DeleteDocumentSendList(IContext context, int sendListId);

        InternalDocument AddDocumentSendListStagePrepare(IContext context, int documentId);

        void ChangeDocumentSendListStage(IContext context, IEnumerable<InternalDocumentSendLists> model);
    }
}