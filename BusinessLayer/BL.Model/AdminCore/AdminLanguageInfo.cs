using System.Collections.Generic;
using BL.Model.AdminCore.InternalModel;

namespace BL.Model.AdminCore
{
    public class AdminLanguageInfo
    {

        public AdminLanguageInfo()
        {
            Languages = new List<InternalAdminLanguage>();
            LanguageValues = new List<InternalAdminLanguageValue>();
        }

        public List<InternalAdminLanguage> Languages { get; set; }
        public List<InternalAdminLanguageValue> LanguageValues { get; set; }
    }
}