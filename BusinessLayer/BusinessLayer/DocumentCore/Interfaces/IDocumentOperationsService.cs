using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;


namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentOperationsService
    {
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId);

        void AddDocumentComment(IContext context, AddNote note);

        void ChangeDocumentWorkStatus(IContext context, ChangeWorkStatus newStatus);

        void ChangeFavouritesForDocument(IContext context, ChangeFavourites model);

        void ControlOn(IContext context, ControlOn model);
        void ControlChange(IContext context, ControlChange model);
        void ControlOff(IContext context, ControlOff model);
        

        void AddDocumentLink(IContext cxt, AddDocumentLink model);
    }
}