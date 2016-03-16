using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;

namespace BL.Database.SystemDb
{
    public interface ISystemDbProcess
    {
        int AddLog(IContext ctx, LogInfo log);
        int AddSetting(IContext ctx, string name, string value, int? agentId = null);
        string GetSettingValue(IContext ctx, string name, int? agentId = null);
        IEnumerable<InternalSystemAction> GetSystemActions(IContext ctx, FilterSystemAction filter);
        IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter);

        #region SystemObjects

        IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter);


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
    }
}