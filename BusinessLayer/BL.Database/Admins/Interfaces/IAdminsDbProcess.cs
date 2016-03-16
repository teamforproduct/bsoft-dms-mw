using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Users;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.AdminCore.FilterModel;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        AdminAccessInfo GetAdminAccesses(IContext context);
        IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);
        //bool VerifyAccess(IContext context, VerifyAccess acc, bool isThrowExeception = true);

        Employee GetEmployee(IContext ctx, int id);
        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);

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
        void UpdateAdminLanguageValue(IContext context, InternalAdminLanguageValue model);
        void DeleteAdminLanguageValue(IContext context, InternalAdminLanguageValue model);
        #endregion AdminLanguageValues
    }
}