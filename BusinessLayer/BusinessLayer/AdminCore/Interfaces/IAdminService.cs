using BL.Model.AdminCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using BL.Model.Users;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface IAdminService
    {
        IEnumerable<BaseAdminUserRole> GetPositionsByCurrentUser(IContext context);
        bool VerifyAccess(IContext context, VerifyAccess verifyAccess, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDocumentActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDictionaryActions action, bool isPositionFromContext = true, bool isThrowExeception = true);

        bool VerifyAccess(IContext context, EnumTemplateDocumentsActions action, bool isPositionFromContext = true,
            bool isThrowExeception = true);
        Employee GetEmployee(IContext context, string userId);
        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);

        #region AdminLanguages
        FrontAdminLanguage GetAdminLanguage(IContext context, int id);
        IEnumerable<FrontAdminLanguage> GetAdminLanguages(IContext context, FilterAdminLanguage filter);
        #endregion AdminLanguages

        #region AdminLanguageValues
        FrontAdminLanguageValue GetAdminLanguageValue(IContext context, int id);
        IEnumerable<FrontAdminLanguageValue> GetAdminLanguageValues(IContext context, FilterAdminLanguageValue filter);
        string ReplaceLanguageLabel(IContext context, string text);
        #endregion AdminLanguageValues
    }
}
