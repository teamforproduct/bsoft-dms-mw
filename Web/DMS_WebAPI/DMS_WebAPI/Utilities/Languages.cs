using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Database;
using DMS_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Represent functionality to configure languages
    /// </summary>
    public class Languages
    {
        private const int _MINUTES_TO_UPDATE_INFO = 5;

        private StoreInfo _language;

        private AdminLanguageInfo GetAdminLanguage()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var res = new AdminLanguageInfo();

                res.Languages = dbContext.AdminLanguagesSet.Select(x => new InternalAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).ToList();

                res.LanguageValues = dbContext.AdminLanguageValuesSet.Select(x => new InternalAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).ToList();

                return res;
            }
        }
        private AdminLanguageInfo GetLanguageInfo()
        {
            if (_language != null)
            {
                if ((DateTime.Now - _language.LastUsage).TotalMinutes > _MINUTES_TO_UPDATE_INFO)
                {
                    var lst = GetAdminLanguage();
                    _language.StoreObject = lst;
                    _language.LastUsage = DateTime.Now;
                    return lst;
                }
                return _language.StoreObject as AdminLanguageInfo;
            }
            var nlst = GetAdminLanguage();
            var nso = new StoreInfo
            {
                LastUsage = DateTime.Now,
                StoreObject = nlst
            };
            _language = nso;
            return nlst;
        }
        private List<InternalAdminLanguageValue> GetLanguageValues(FilterAdminLanguageValue filter)
        {
            var languageInfo = GetLanguageInfo();

            if (string.IsNullOrEmpty(filter.LanguageName))
                filter.LanguageName = string.Empty;
            var language = languageInfo.Languages.FirstOrDefault(x => filter.LanguageName.Equals(x.Code, StringComparison.OrdinalIgnoreCase));
            if (language == null)
            {
                language = languageInfo.Languages.FirstOrDefault(x => x.IsDefault);
            }

            var languageValues = languageInfo.LanguageValues
                .Where(x => x.LanguageId == language.Id)
                .Where(x => filter.Labels.Contains(x.Label))
                .ToList();

            return languageValues;
        }

        public string ReplaceLanguageLabel(string userLanguage, string text)
        {
            string errorMessage = text;

            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var labelsInText = new List<string>();
                    foreach (Match label in Regex.Matches(errorMessage, "##l@(.*?)@l##"))
                    {
                        labelsInText.Add(label.Value);
                    }

                    if (labelsInText.Count > 0)
                    {
                        if (string.IsNullOrEmpty(userLanguage))
                            userLanguage = string.Empty;

                        var labels = GetLanguageValues(new FilterAdminLanguageValue { LanguageName = userLanguage, Labels = labelsInText }).ToArray();

                        for (int i = 0, l = labels.Length; i < l; i++)
                        {
                            errorMessage = errorMessage.Replace(labels[i].Label, labels[i].Value);
                        }
                    }

                    // Еслли в переводе пусто...
                    if (errorMessage == string.Empty) errorMessage = "Empty translation for label: " + text;

                    return errorMessage;
                }
            }
            catch (Exception ex) { }
            return text;
        }
    }
}