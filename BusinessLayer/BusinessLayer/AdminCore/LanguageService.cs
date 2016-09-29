using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.AdminCore.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using BL.CrossCutting.Interfaces;
using BL.Logic.Common;
using BL.Model.Database;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using System.Text.RegularExpressions;
using BL.Model.AdminCore.InternalModel;
using BL.Database.DatabaseContext;

namespace BL.Logic.AdminCore
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguagesDbProcess _languageDb;

        private const int _MINUTES_TO_UPDATE_INFO = 5;

        private Dictionary<string, StoreInfo> languageList;

        public LanguageService(ILanguagesDbProcess languageDb)
        {
            _languageDb = languageDb;
            languageList = new Dictionary<string, StoreInfo>();
        }

        private AdminLanguageInfo GetLanguageInfo(IContext context)
        {
            var key = CommonSystemUtilities.GetServerKey(context);
            if (languageList.ContainsKey(key))
            {
                var so = languageList[key];
                if ((DateTime.Now - so.LastUsage).TotalMinutes > _MINUTES_TO_UPDATE_INFO)
                {
                    var lst = _languageDb.GetAdminLanguage(context);
                    so.StoreObject = lst;
                    so.LastUsage = DateTime.Now;
                    return lst;
                }
                return so.StoreObject as AdminLanguageInfo;
            }
            var nlst = _languageDb.GetAdminLanguage(context);
            var nso = new StoreInfo
            {
                LastUsage = DateTime.Now,
                StoreObject = nlst
            };
            languageList.Add(key, nso);
            return nlst;
        }
        private List<InternalAdminLanguageValue> GetLanguageValues(IContext context, FilterAdminLanguageValue filter)
        {
            var languageInfo = GetLanguageInfo(context);

            var language = languageInfo.Languages.FirstOrDefault(x => filter.LanguageId > 0 ? x.Id == filter.LanguageId : x.IsDefault);

            var languageValues = languageInfo.LanguageValues
                .Where(x => x.LanguageId == language.Id)
                .Where(x => filter.Labels.Contains(x.Label))
                .ToList();

            return languageValues;
        }
        public string ReplaceLanguageLabel(IContext context, string text)
        {
            var labelsInText = new List<string>();
            foreach (Match label in Regex.Matches(text, "##l@(.*?)@l##"))
            {
                labelsInText.Add(label.Value);
            }

            if (labelsInText.Count > 0)
            {
                var labels = GetLanguageValues(context, new FilterAdminLanguageValue { LanguageId = context.CurrentEmployee.LanguageId, Labels = labelsInText }).ToArray();

                for (int i = 0, l = labels.Length; i < l; i++)
                {
                    text = text.Replace(labels[i].Label, labels[i].Value);
                }
            }
            return text;
        }

        #region AdminLanguages
        public FrontAdminLanguage GetAdminLanguage(IContext context, int id)
        {
            return _languageDb.GetAdminLanguages(context, new FilterAdminLanguage { LanguageId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAdminLanguage> GetAdminLanguages(IContext context, FilterAdminLanguage filter)
        {
            return _languageDb.GetAdminLanguages(context, filter);
        }
        #endregion AdminLanguages

        #region AdminLanguageValues
        public FrontAdminLanguageValue GetAdminLanguageValue(IContext context, int id)
        {
            return _languageDb.GetAdminLanguageValues(context, new FilterAdminLanguageValue { LanguageValueId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAdminLanguageValue> GetAdminLanguageValues(IContext context, FilterAdminLanguageValue filter)
        {
            return _languageDb.GetAdminLanguageValues(context, filter);
        }

        public void RefreshLanguageValues(IContext context)
        {
            _languageDb.DeleteAllAdminLanguageValues(context);
            //_languageDb.AddAdminLanguageValues(context, DmsDbImportData.GetAdminLanguageValues());
        }

        #endregion AdminLanguageValues
    }
}
