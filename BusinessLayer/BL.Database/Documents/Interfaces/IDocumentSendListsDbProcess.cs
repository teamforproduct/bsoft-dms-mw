using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentSendListsDbProcess
    {
        ModifyDocumentRestrictedSendList GetRestrictedSendListById(IContext ctx, int id);
        BaseDocumentRestrictedSendList GetRestrictedSendListBaseById(IContext ctx, int id);
        IEnumerable<ModifyDocumentRestrictedSendList> GetRestrictedSendList(IContext ctx, int documentId);
        IEnumerable<BaseDocumentRestrictedSendList> GetRestrictedSendListBase(IContext ctx, int documentId);
        void UpdateRestrictedSendList(IContext ctx, ModifyDocumentRestrictedSendList restrictedSendList);
        IEnumerable<int> AddRestrictedSendList(IContext ctx, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists);
        void DeleteRestrictedSendList(IContext ctx, int restrictedSendListId);
        IEnumerable<ModifyDocumentSendList> GetSendList(IContext ctx, int documentId);
        IEnumerable<BaseDocumentSendList> GetSendListBase(IContext ctx, int documentId);
        ModifyDocumentSendList GetSendListById(IContext ctx, int id);
        BaseDocumentSendList GetSendListBaseById(IContext ctx, int id);
        void UpdateSendList(IContext ctx, ModifyDocumentSendList sendList);
        IEnumerable<int> AddSendList(IContext ctx, IEnumerable<ModifyDocumentSendList> sendLists);
        void DeleteSendList(IContext ctx, int sendListId);
    }
}