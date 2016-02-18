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
        void UpdateRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList);
        int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList);
        void AddRestrictedSendListByStandartSendLists(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model);
        void DeleteRestrictedSendList(IContext context, int restrictedSendListId);
        void ValidSendListsBySendLists(IContext context, int documentId, IEnumerable<ModifyDocumentSendList> sendLists);
        void ValidSendListsByRestrictedSendLists(IContext context, int documentId, IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists);
        void ValidSendLists(IContext context, int documentId, IEnumerable<ModifyDocumentSendList> sendLists            , IEnumerable<ModifyDocumentRestrictedSendList> restrictedSendLists);
        IEnumerable<FrontDocumentSendListStage> GetSendListStage(IContext context, int documentId, bool isLastStage = false);
        IEnumerable<FrontDocumentSendListStage> GetSendListStage(IEnumerable<FrontDocumentSendList> sendLists);
        FrontDocumentSendList GetSendList(IContext context, int sendListId);
        IEnumerable<FrontDocumentSendList> GetSendLists(IContext context, int documentId);
        void UpdateSendList(IContext context, ModifyDocumentSendList sendList);
        int AddSendList(IContext context, ModifyDocumentSendList sendList);
        void AddSendListByStandartSendLists(IContext context, ModifyDocumentSendListByStandartSendList model);
        void DeleteSendList(IContext context, int sendListId);
        bool AddSendListStage(IContext context, ModifyDocumentSendListStage model);
        void DeleteSendListStage(IContext context, ModifyDocumentSendListStage model);
    }
}