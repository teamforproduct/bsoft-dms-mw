using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentSendListsDbProcess
    {
        FrontDocumentRestrictedSendList GetRestrictedSendList(IContext ctx, int id);
        IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendLists(IContext ctx, int documentId);
        IEnumerable<FrontDocumentSendList> GetSendLists(IContext ctx, int documentId);
        FrontDocumentSendList GetSendList(IContext ctx, int id);
    }
}