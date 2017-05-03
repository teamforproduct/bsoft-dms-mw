using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface ILanguages
    {
        void RefreshLanguageValues();

        string GetTranslation(int languageId, string text);
        string GetTranslation(string languageName, string text);
        string GetTranslation(IContext Context, string text);
        IEnumerable<InternalAdminLanguage> GetLanguages(FilterAdminLanguage filter);
        IEnumerable<InternalAdminLanguageValue> GetLanguageValues(FilterAdminLanguageValue filter);
        string GetTranslation(string text);

        int GetLanguageIdByCode(string languageCode);

        int GetLanguageIdByHttpContext();

        InternalAdminLanguage GetDefaultLanguage();

        string GetLabel(string module, string item);
    }
}
