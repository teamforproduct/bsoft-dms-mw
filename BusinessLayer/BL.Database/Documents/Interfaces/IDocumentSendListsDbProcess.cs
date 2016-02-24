using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentSendListsDbProcess
    {
        FrontDocumentRestrictedSendList GetRestrictedSendListBaseById(IContext ctx, int id);
        IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId);
        IEnumerable<FrontDocumentSendList> GetSendListBase(IContext ctx, int documentId);
        FrontDocumentSendList GetSendListBaseById(IContext ctx, int id);
    }
}