using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using System.Web;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface ILanguages
    {
        string ReplaceLanguageLabel(int languageId, string text);
        string ReplaceLanguageLabel(string languageName, string text);
        string ReplaceLanguageLabel(HttpContext Context, string text);
        string ReplaceLanguageLabel(IContext Context, string text);


    }
}
