using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;

namespace BL.Database.SystemDb
{
    public interface ISystemDbProcess
    {
        void InitializerDatabase(IContext context);

        #region Logging
        int AddLog(IContext ctx, LogInfo log);
        #endregion

        #region Settings
        int AddSetting(IContext ctx, string name, string value, int? agentId = null);
        string GetSettingValue(IContext ctx, string name, int? agentId = null);
        #endregion

        #region SystemObjects
        IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter);
        IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter);
        #endregion SystemObjects

        #region Properties
        IEnumerable<BaseSystemUIElement> GetPropertyUIElements(IContext context, FilterPropertyLink filter);

        IEnumerable<FrontProperty> GetProperties(IContext context, FilterProperty filter);

        int AddProperty(IContext context, InternalProperty model);

        void UpdateProperty(IContext context, InternalProperty model);

        void DeleteProperty(IContext context, InternalProperty model);
        #endregion Properties

        #region PropertyLinks

        IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext context, FilterPropertyLink filter);

        IEnumerable<InternalPropertyLink> GetInternalPropertyLinks(IContext context, FilterPropertyLink filter);

        int AddPropertyLink(IContext context, InternalPropertyLink model);

        void UpdatePropertyLink(IContext context, InternalPropertyLink model);

        void DeletePropertyLink(IContext context, InternalPropertyLink model);

        #endregion PropertyLinks

        IEnumerable<FrontPropertyValue> GetPropertyValuesToDocumentFromTemplateDocument(IContext context, FilterPropertyLink filter);

        #region Mailing
        IEnumerable<InternalDataForMail> GetNewActionsForMailing(IContext ctx);
        void MarkActionsLikeMailSended(IContext ctx, InternalMailProcessed mailProcessed);
        #endregion

        #region Filter Properties
        IEnumerable<BaseSystemUIElement> GetFilterProperties(IContext context, FilterProperties filter);
        #endregion Filter Properties

        #region AutoPlan

        IEnumerable<int> GetSendListIdsForAutoPlan(IContext context, int? sendListId = null, int? documentId = null);

        #endregion

        IEnumerable<int> GetDocumentIdsForClearTrashDocuments(IContext context, int timeMinForClearTrashDocuments);

        #region Full text search

        int GetEntityNumbers(IContext ctx, EnumObjects objType);
        int GetCurrentMaxCasheId(IContext ctx);
        IEnumerable<FullTextIndexItem> FullTextIndexOneDocumentReindexDbPrepare(IContext ctx, int selectBis);
        IEnumerable<FullTextIndexItem> FullTextIndexDocumentsReindexDbPrepare(IContext ctx, EnumObjects objType, int rowToSelect, int rowOffset);
        IEnumerable<FullTextIndexItem> FullTextIndexNonDocumentsReindexDbPrepare(IContext ctx);
        IEnumerable<FullTextIndexItem> FullTextIndexDocumentsPrepare(IContext ctx, EnumObjects objType, int rowToSelect, int selectBis);
        IEnumerable<FullTextIndexItem> FullTextIndexNonDocumentsPrepare(IContext ctx);
        IEnumerable<FullTextIndexItem> FullTextIndexToDeletePrepare(IContext ctx);
        void FullTextIndexDeleteProcessed(IContext ctx, IEnumerable<int> processedIds, bool deleteSimilarObject = false);
        void DeleteRelatedToDocumentRecords(IContext ctx, IEnumerable<int> docIds, int? deleteBis = null);
        void FullTextIndexDeleteCash(IContext ctx, int deleteBis);

        #endregion
    }
}