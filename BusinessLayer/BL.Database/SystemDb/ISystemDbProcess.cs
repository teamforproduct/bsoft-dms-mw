using BL.Model.SystemCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore.InternalModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Database.DBModel.System;
using System;
using BL.Model.DictionaryCore.FrontModel.Employees;

namespace BL.Database.SystemDb
{
    public interface ISystemDbProcess
    {
        void InitializerDatabase(IContext context);

        #region Logging
        FrontAgentEmployeeUser GetLastSuccessLoginInfo(IContext context);
        FrontAgentEmployeeUser GetLastErrorLoginInfo(IContext context, DateTime? dateFrom);
        void DeleteSystemLogs(IContext context, FilterSystemLog filter);
        IEnumerable<FrontSystemLog> GetSystemLogs(IContext context, FilterSystemLog filter, UIPaging paging);
        IEnumerable<FrontSearchQueryLog> GetSystemSearchQueryLogs(IContext context, FilterSystemSearchQueryLog filter, UIPaging paging);
        void DeleteSystemSearchQueryLogsForCurrentUser(IContext context, FilterSystemSearchQueryLog filter);
        void DeleteSystemSearchQueryLogs(IContext ctx, FilterSystemSearchQueryLog filter);
        int AddSearchQueryLog(IContext ctx, InternalSearchQueryLog model);
        int AddLog(IContext ctx, InternalLog log);
        void UpdateLogDate1(IContext ctx, int id, DateTime datetime);
        #endregion

        #region Settings
        int MergeSetting(IContext ctx, InternalSystemSetting model);
        void DeleteSystemSettings(IContext ctx);
        string GetSettingValue(IContext ctx, FilterSystemSetting filter);
        IEnumerable<InternalSystemSetting> GetSystemSettingsInternal(IContext ctx, FilterSystemSetting filter);
        IEnumerable<FrontSystemSetting> GetSystemSettings(IContext ctx, FilterSystemSetting filter);
        #endregion

        #region [+] SystemObjects ...
        void AddSystemObject(IContext context, InternalSystemObject item);
        void UpdateSystemObject(IContext context, InternalSystemObject item);
        void AddSystemObject(IContext context, SystemObjects item);
        void UpdateSystemObject(IContext context, SystemObjects item);
        void DeleteSystemObjects(IContext context, FilterSystemObject filter);
        IEnumerable<FrontSystemObject> GetSystemObjects(IContext context, FilterSystemObject filter);
        IEnumerable<int> GetObjectsByActions(IContext context, FilterSystemAction filter);
        #endregion

        #region [+] Actions ...
        void AddSystemAction(IContext context, InternalSystemAction item);
        void UpdateSystemAction(IContext context, InternalSystemAction item);
        void AddSystemAction(IContext context, SystemActions item);
        void UpdateSystemAction(IContext context, SystemActions item);
        void DeleteSystemActions(IContext context, FilterSystemAction filter);
        IEnumerable<InternalSystemAction> GetInternalSystemActions(IContext ctx, FilterSystemAction filter);
        IEnumerable<FrontSystemAction> GetSystemActions(IContext context, FilterSystemAction filter);
        #endregion

        #region [+] Permissions
        IEnumerable<InternalPermissions> GetInternalPermissions(IContext ctx, FilterSystemPermissions filter);
        int GetPermissionId(IContext context, string module, string feture, string accessType);
        #endregion

        IEnumerable<FrontSystemFormat> GetSystemFormats(IContext context, FilterSystemFormat filter);
        IEnumerable<FrontSystemFormula> GetSystemFormulas(IContext context, FilterSystemFormula filter);
        IEnumerable<FrontSystemPattern> GetSystemPatterns(IContext context, FilterSystemPattern filter);
        IEnumerable<FrontSystemValueType> GetSystemValueTypes(IContext context, FilterSystemValueType filter);
        IEnumerable<BaseSystemUIElement> GetSystemUIElements(IContext ctx, FilterSystemUIElement filter);

        #region Properties
        IEnumerable<BaseSystemUIElement> GetPropertyUIElements(IContext context, FilterPropertyLink filter);

        IEnumerable<FrontProperty> GetProperties(IContext context, FilterProperty filter);

        int AddProperty(IContext context, InternalProperty model);

        void UpdateProperty(IContext context, InternalProperty model);

        void DeleteProperties(IContext context, FilterProperty filter);
        #endregion Properties

        #region PropertyLinks

        IEnumerable<FrontPropertyLink> GetPropertyLinks(IContext context, FilterPropertyLink filter);

        IEnumerable<InternalPropertyLink> GetInternalPropertyLinks(IContext context, FilterPropertyLink filter);

        int AddPropertyLink(IContext context, InternalPropertyLink model);

        void UpdatePropertyLink(IContext context, InternalPropertyLink model);

        void DeletePropertyLinks(IContext context, FilterPropertyLink filter);

        #endregion PropertyLinks

        IEnumerable<FrontPropertyValue> GetPropertyValuesToDocumentFromTemplate(IContext context, FilterPropertyLink filter);

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

        void RefreshModuleFeature(IContext context);

    }
}