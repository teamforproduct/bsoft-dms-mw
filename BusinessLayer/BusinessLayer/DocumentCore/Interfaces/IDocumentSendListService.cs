using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentSendListService
    {
        FrontDocumentRestrictedSendList GetRestrictedSendList(IContext context, int restrictedSendListId);
        IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendLists(IContext context, int documentId);
        IEnumerable<FrontDocumentSendListStage> GetSendListStage(IContext context, int documentId, bool isLastStage = false);
        FrontDocumentSendList GetSendList(IContext context, int sendListId);
        IEnumerable<FrontDocumentSendList> GetSendLists(IContext context, int documentId);
    }
}