using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.AdminCore.FilterModel;
using BL.Database.DBModel.Admin;

namespace BL.Database.Admins.Interfaces
{
    public interface ILanguagesDbProcess
    {
        AdminLanguageInfo GetAdminLanguage(IContext context);

        #region AdminLanguages
        IEnumerable<FrontAdminLanguage> GetAdminLanguages(IContext context, FilterAdminLanguage filter);
        InternalAdminLanguage GetInternalAdminLanguage(IContext context, FilterAdminLanguage filter);
        int AddAdminLanguage(IContext context, InternalAdminLanguage model);
        void UpdateAdminLanguage(IContext context, InternalAdminLanguage model);
        void DeleteAdminLanguage(IContext context, InternalAdminLanguage model);
        #endregion AdminLanguages

        #region AdminLanguageValues
        IEnumerable<FrontAdminLanguageValue> GetAdminLanguageValues(IContext context, FilterAdminLanguageValue filter);
        InternalAdminLanguageValue GetInternalAdminLanguageValue(IContext context, FilterAdminLanguageValue filter);
        int AddAdminLanguageValue(IContext context, InternalAdminLanguageValue model);
        void AddAdminLanguageValues(IContext context, List<AdminLanguageValues> list);
        void UpdateAdminLanguageValue(IContext context, InternalAdminLanguageValue model);
        void DeleteAdminLanguageValue(IContext context, InternalAdminLanguageValue model);
        void DeleteAllAdminLanguageValues(IContext context);
        #endregion AdminLanguageValues
    }
}