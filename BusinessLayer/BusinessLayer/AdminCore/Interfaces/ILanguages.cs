using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore.FilterModel;
using System.Web;
using BL.Model.AdminCore.InternalModel;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface ILanguages
    {
        void RefreshLanguageValues();

        string ReplaceLanguageLabel(int languageId, string text);
        string ReplaceLanguageLabel(string languageName, string text);
        string ReplaceLanguageLabel(HttpContext Context, string text);
        string ReplaceLanguageLabel(IContext Context, string text);
        IEnumerable<InternalAdminLanguage> GetLanguages(FilterAdminLanguage filter);
        IEnumerable<InternalAdminLanguageValue> GetLanguageValues(FilterAdminLanguageValue filter);
        string GetTranslation(string text);

        int GetLanguageIdByCode(string languageCode);

        int GetLanguageIdByHttpContext();
    }
}
