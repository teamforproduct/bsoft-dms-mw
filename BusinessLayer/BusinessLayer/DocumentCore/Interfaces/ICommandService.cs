using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface ICommandService
    {
        object ExecuteCommand(ICommand cmd);
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentActions(IContext ctx, int documentId);
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentSendListActions(IContext ctx, int id);
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentFileActions(IContext ctx, int documentId);
        IEnumerable<InternalDictionaryPositionWithActions> GetDocumentPaperActions(IContext ctx, int id);
    }
}