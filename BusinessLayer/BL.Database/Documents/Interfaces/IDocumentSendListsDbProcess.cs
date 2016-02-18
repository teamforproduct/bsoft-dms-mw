using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentSendListsDbProcess
    {
        ModifyDocumentRestrictedSendList GetRestrictedSendListById(IContext ctx, int id);
        FrontDocumentRestrictedSendList GetRestrictedSendListBaseById(IContext ctx, int id);
        IEnumerable<ModifyDocumentRestrictedSendList> GetRestrictedSendList(IContext ctx, int documentId);
        IEnumerable<FrontDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId);
        void UpdateRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList);
        IEnumerable<int> AddRestrictedSendList(IContext ctx, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists);
        void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId);
        IEnumerable<ModifyDocumentSendList> GetSendList(IContext ctx, int documentId);
        IEnumerable<FrontDocumentSendList> GetSendListBase(IContext ctx, int documentId);
        ModifyDocumentSendList GetSendListById(IContext ctx, int id);
        FrontDocumentSendList GetSendListBaseById(IContext ctx, int id);
        void UpdateSendList(IContext ctx, ModifyDocumentSendList sendList);
        IEnumerable<int> AddSendList(IContext ctx, IEnumerable<ModifyDocumentSendList> sendLists);
        void DeleteSendList(IContext ctx, int sendListId);
    }
}