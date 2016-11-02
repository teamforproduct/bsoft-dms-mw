using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface ILanguageService
    {
        #region AdminLanguages
        FrontAdminLanguage GetAdminLanguage(IContext context, int id);
        IEnumerable<FrontAdminLanguage> GetAdminLanguages(IContext context, FilterAdminLanguage filter);
        IEnumerable<FrontAdminUserLanguage> GetAdminUserLanguages(IContext context, int userId, FilterAdminLanguage filter);
        #endregion AdminLanguages

        #region AdminLanguageValues
        FrontAdminLanguageValue GetAdminLanguageValue(IContext context, int id);
        IEnumerable<FrontAdminLanguageValue> GetAdminLanguageValues(IContext context, FilterAdminLanguageValue filter);
        //string ReplaceLanguageLabel(IContext context, string text);
        void RefreshLanguageValues(IContext context);
        #endregion AdminLanguageValues
    }
}
