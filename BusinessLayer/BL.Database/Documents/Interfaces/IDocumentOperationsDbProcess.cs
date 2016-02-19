using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.InternalModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentOperationsDbProcess
    {
        bool VerifyDocumentRegistrationNumber(IContext ctx, RegisterDocument registerDocument);
        bool SetDocumentRegistration(IContext ctx, RegisterDocument registerDocument);
        void AddDocumentLink(IContext context, InternalLinkedDocument model);

        FrontDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId);
        void AddDocumentWait(IContext ctx, FrontDocumentWaits documentWait);
        void UpdateDocumentWait(IContext ctx, FrontDocumentWaits documentWait);

        int AddDocumentEvent(IContext ctx, FrontDocumentEvent docEvent);
        IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter);

        int AddDocumentAccess(IContext ctx, FrontDocumentAccess access);
        void RemoveDocumentAccess(IContext ctx, int accessId);
        void UpdateDocumentAccess(IContext ctx, FrontDocumentAccess access);
        FrontDocumentAccess GetDocumentAccess(IContext ctx, int documentId);

        void SetDocumentInformation(IContext ctx, EventAccessModel access);
        InternalLinkedDocument AddDocumentLinkPrepare(IContext context, AddDocumentLink model);

    }
}