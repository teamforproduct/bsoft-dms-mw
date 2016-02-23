using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentOperationsDbProcess
    {
        void AddDocumentLink(IContext context, InternalLinkedDocument model);

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

        InternalDocumentAccesses ChangeIsInWorkAccessPrepare(IContext context, int documentId);
        InternalDocumentAccesses ChangeIsFavouriteAccessPrepare(IContext context, int documentId);
        IEnumerable<InternalDocumentAccesses> GetDocumentAccesses(IContext ctx, int documentId);

        void ChangeIsInWorkAccess(IContext ctx, InternalDocumentAccesses access);
        InternalLinkedDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model);
        InternalDocument GetDocumentActionsPrepare(IContext context, int documentId);
        void ChangeIsFavouriteAccess(IContext context, InternalDocumentAccesses docAccess);
    }
}