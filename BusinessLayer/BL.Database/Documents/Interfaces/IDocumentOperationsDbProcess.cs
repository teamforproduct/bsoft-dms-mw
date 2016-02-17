using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Actions;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentOperationsDbProcess
    {
        bool VerifyDocumentRegistrationNumber(IContext ctx, RegisterDocument registerDocument);
        bool SetDocumentRegistration(IContext ctx, RegisterDocument registerDocument);
        void AddDocumentLink(IContext context, ВaseDocumentLink model);

        BaseDocumentWaits GetDocumentWaitByOnEventId(IContext ctx, int eventId);
        void AddDocumentWait(IContext ctx, BaseDocumentWaits documentWait);
        void UpdateDocumentWait(IContext ctx, BaseDocumentWaits documentWait);

        int AddDocumentEvent(IContext ctx, BaseDocumentEvent docEvent);
        IEnumerable<BaseDocumentEvent> GetDocumentEvents(IContext ctx, FilterDocumentEvent filter);

        int AddDocumentAccess(IContext ctx, BaseDocumentAccess access);
        void RemoveDocumentAccess(IContext ctx, int accessId);
        void UpdateDocumentAccess(IContext ctx, BaseDocumentAccess access);
        BaseDocumentAccess GetDocumentAccess(IContext ctx, int documentId);

        void SetDocumentInformation(IContext ctx, EventAccessModel access);

    }
}