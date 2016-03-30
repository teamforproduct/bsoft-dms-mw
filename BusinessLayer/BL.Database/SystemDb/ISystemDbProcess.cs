using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.FullTextSerach;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;

namespace BL.Database.SystemDb
{
    public interface ISystemDbProcess
    {
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
        InternalProperty GetProperty(IContext context, FilterProperty filter);

        IEnumerable<FrontProperty> GetProperties(IContext context, FilterProperty filter);

        int AddProperty(IContext context, InternalProperty model);

        void UpdateProperty(IContext context, InternalProperty model);

        void DeleteProperty(IContext context, InternalProperty model);
        #endregion Properties

        #region PropertyLinks
        InternalPropertyLink GetPropertyLink(IContext context, FilterPropertyLink filter);

        IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext context, FilterPropertyLink filter);

        int AddPropertyLink(IContext context, InternalPropertyLink model);

        void UpdatePropertyLink(IContext context, InternalPropertyLink model);

        void DeletePropertyLink(IContext context, InternalPropertyLink model);

        #endregion PropertyLinks

        #region Mailing
        IEnumerable<InternalDataForMail> GetNewActionsForMailing(IContext ctx);
        void MarkActionsLikeMailSended(IContext ctx, InternalMailProcessed mailProcessed);
        #endregion

        #region Filter Properties
        IEnumerable<BaseSystemUIElement> GetFilterProperties(IContext context, FilterProperties filter);
        #endregion Filter Properties

        #region AutoPlan

        IEnumerable<int> GetSendListIdsForAutoPlan(IContext context);

        #endregion

        #region Full text search
        IEnumerable<FullTextIndexIem> FullTextIndexReindexDbPrepare(IContext ctx);
        IEnumerable<FullTextIndexIem> FullTextIndexPrepare(IContext ctx);
        void FullTextIndexDeleteProcessed(IContext ctx, IEnumerable<int> processedIds);

        #endregion
    }
}