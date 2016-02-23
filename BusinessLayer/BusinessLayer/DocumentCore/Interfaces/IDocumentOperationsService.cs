using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;


namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentOperationsService
    {
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId);
    }
}