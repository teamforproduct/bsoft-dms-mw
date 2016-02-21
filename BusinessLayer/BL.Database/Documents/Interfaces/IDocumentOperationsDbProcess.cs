using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentOperationsDbProcess
    {
        void AddDocumentLink(IContext context, InternalLinkedDocument model);

        InternalDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId);
        void AddDocumentWait(IContext ctx, InternalDocumentWaits documentWait);
        void UpdateDocumentWait(IContext ctx, InternalDocumentWaits documentWait);

        int AddDocumentEvent(IContext ctx, InternalDocumentEvents docEvent);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter);

        int AddDocumentAccess(IContext ctx, InternalDocumentAccesses access);
        void RemoveDocumentAccess(IContext ctx, int accessId);
        void UpdateDocumentAccess(IContext ctx, InternalDocumentAccesses access);
        InternalDocumentAccesses GetDocumentAccess(IContext ctx, int documentId);

        void SetDocumentInformation(IContext ctx, EventAccessModel access);
        InternalLinkedDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model);
        InternalDocument GetDocumentActionsPrepare(IContext context, int documentId);
    }
}