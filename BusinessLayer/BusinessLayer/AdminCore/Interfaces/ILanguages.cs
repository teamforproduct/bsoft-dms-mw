using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface ILanguages
    {
        void RefreshLanguageValues();

        string GetTranslation(int languageId, string text, List<string> Paramenters = null);
        string GetTranslation(string languageName, string text, List<string> Paramenters = null);
        string GetTranslation(IContext Context, string text);
        IEnumerable<InternalAdminLanguage> GetLanguages(FilterAdminLanguage filter);
        string GetTranslation(string text, List<string> Paramenters = null);

        int GetLanguageIdByCode(string languageCode);

        int GetLanguageIdByHttpContext();

        InternalAdminLanguage GetDefaultLanguage();
    }
}
