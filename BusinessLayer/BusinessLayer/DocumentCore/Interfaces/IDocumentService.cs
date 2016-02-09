﻿using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Actions;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentService
    {
        int SaveDocument(IContext context, FullDocument document);

        int AddDocumentByTemplateDocument(IContext context, int templateDocumentId);
        int ModifyDocument(IContext context, ModifyDocument document);

        IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging);
        FullDocument GetDocument(IContext ctx, int documentId);

        IEnumerable<BaseSystemUIElement> GetModifyMetaData(IContext cxt, FullDocument doc);

        void UpdateRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList);
        int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList);
        void AddRestrictedSendListByStandartSendLists(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model);
        void DeleteRestrictedSendList(IContext context, int restrictedSendListId);

        void UpdateSendList(IContext context, ModifyDocumentSendList sendList);
        int AddSendList(IContext ctx, ModifyDocumentSendList sendList);
        void AddSendListByStandartSendLists(IContext context, ModifyDocumentSendListByStandartSendList model);
        void DeleteSendList(IContext context, int sendListId);

        int SaveSavedFilter(IContext context, ModifyDocumentSavedFilter savedFilter);
        IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx);
        BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId);
        void DeleteSavedFilter(IContext context, int savedFilterId);

        int AddDocumentAccess(IContext ctx, BaseDocumentAccess access);
        void RemoveDocumentAccess(IContext ctx, int accessId);

        IEnumerable<BaseSystemAction> GetDocumentActions(IContext ctx, int documentId);

        void AddDocumentComment(IContext context, AddNote note);

        void ChangeDocumentWorkStatus(IContext context, ChangeWorkStatus newStatus);

        void ChangeFavouritesForDocument(IContext context, ChangeFavourites model);

        void ControlOn(IContext context, ControlOn model);

        int CopyDocument(IContext context, CopyDocument model);
        void RegisterDocument(IContext context, RegisterDocument model);
    }
}