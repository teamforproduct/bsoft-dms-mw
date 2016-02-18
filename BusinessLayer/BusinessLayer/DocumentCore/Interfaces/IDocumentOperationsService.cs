using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentOperationsService
    {
        IEnumerable<BaseSystemAction> GetDocumentActions(IContext ctx, int documentId);

        void AddDocumentComment(IContext context, AddNote note);

        int AddDocumentAccess(IContext ctx, FrontDocumentAccess access);
        void RemoveDocumentAccess(IContext ctx, int accessId);

        void ChangeDocumentWorkStatus(IContext context, ChangeWorkStatus newStatus);

        void ChangeFavouritesForDocument(IContext context, ChangeFavourites model);

        void ControlOn(IContext context, ControlOn model);
        void ControlChange(IContext context, ControlChange model);
        void ControlOff(IContext context, ControlOff model);
        
        void RegisterDocument(IContext context, RegisterDocument model);
        void AddDocumentLink(IContext cxt, AddDocumentLink model);
    }
}