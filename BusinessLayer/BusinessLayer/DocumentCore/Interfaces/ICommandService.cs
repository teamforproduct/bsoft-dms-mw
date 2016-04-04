using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface ICommandService
    {
        object ExecuteCommand(ICommand cmd);
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId);
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentSendListActions(IContext cxt, int id);
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentPaperActions(IContext cxt, int id);
    }
}