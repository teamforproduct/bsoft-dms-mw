using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using DMS_WebAPI.DatabaseContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Represent functionality to configure languages
    /// </summary>
    public class Languages : ILanguages
    {
        private const int _MINUTES_TO_UPDATE_INFO = int.MaxValue;

        private const string _PATTERN = "##l@(.*?)@l##";

        private StoreInfo _language;

        #region [+] Service

        /// <summary>
        /// Возвращает GetAdminLanguage из кэша
        /// </summary>
        /// <returns></returns>
        private AdminLanguageInfo GetLanguageInfo()
        {
            var sPath = Properties.Settings.Default.ServerPath;
            // если нет кэша - первый вход и после сброса
            if (_language == null)
            {
                var nlst = GetAdminLanguageStruct(sPath);
                var nso = new StoreInfo
                {
                    LastUsage = DateTime.UtcNow,
                    StoreObject = nlst
                };
                _language = nso;
                return nlst;
            }
            else
            {
                if ((DateTime.UtcNow - _language.LastUsage).TotalMinutes > _MINUTES_TO_UPDATE_INFO)
                {
                    var lst = GetAdminLanguageStruct(sPath);
                    _language.StoreObject = lst;
                    _language.LastUsage = DateTime.UtcNow;
                    return lst;
                }
                return _language.StoreObject as AdminLanguageInfo;
            }

        }

        private IEnumerable<InternalAdminLanguageValue> GetLanguageValues(FilterAdminLanguageValue filter)
        {
            // выгребаю все переводы из кэша 
            var languageInfo = GetLanguageInfo();

            // выгребаю переводы для заданных filter.Labels
            var languageValues = languageInfo.LanguageValues.AsQueryable();

            if (filter != null)
            {
                if (filter.LanguageId.HasValue)
                {
                    languageValues = languageValues.Where(x => x.LanguageId == filter.LanguageId);
                }

                if (filter.Labels?.Count > 0)
                {
                    languageValues = languageValues.Where(x => filter.Labels.Contains(x.Label));
                }
            }
            return languageValues.ToList(); ;
        }

        public IEnumerable<InternalAdminLanguage> GetLanguages(FilterAdminLanguage filter)
        {
            // выгребаю все переводы из кэша 
            var languageInfo = GetLanguageInfo();

            // выгребаю переводы для заданных filter.Labels
            var languageValues = languageInfo.Languages.AsQueryable();

            if (filter != null)
            {

                if (filter.IDs?.Count > 0)
                {
                    languageValues = languageValues.Where(x => filter.IDs.Contains(x.Id));
                }

                if (!string.IsNullOrEmpty(filter.Code))
                {
                    languageValues = languageValues.Where(x => x.Code == filter.Code);
                }

                if (filter.IsDefault.HasValue)
                {
                    languageValues = languageValues.Where(x => x.IsDefault == filter.IsDefault);
                }

            }

            return languageValues.ToList();
        }

        private bool ExistsLabels(string text)
        {
            return Regex.IsMatch(text, _PATTERN);
        }

        public string GetTranslation(int languageId, string text, List<string> Paramenters = null)
        {
            if (languageId < 1)
            {
                // запрашиваю из кэша переводы
                var languageInfo = GetLanguageInfo();
                var language = languageInfo.Languages.FirstOrDefault(x => x.IsDefault);
                if (language != null) languageId = language.Id;
            }

            var res = Translate(languageId, text);

            if (Paramenters?.Count() > 0)
            {
                try
                {
                    res = string.Format(res, Paramenters.ToArray());

                    res = Translate(languageId, res);
                }
                catch
                { }
            }

            return res;
        }


        private string Translate(int languageId, string text)
        {
            string errorMessage = text;

            try
            {
                var labelsInText = new List<string>();
                // нахожу в тексте все лейблы, которые нужно переводить
                foreach (Match label in Regex.Matches(errorMessage, _PATTERN))
                {
                    labelsInText.Add(label.Value);
                }

                if (labelsInText.Count > 0)
                {
                    // получаю массив переводов для найденных лейблов
                    var labels = GetLanguageValues(new FilterAdminLanguageValue { LanguageId = languageId, Labels = labelsInText }).ToArray();

                    // заменяю лейблы на переводы
                    for (int i = 0, l = labels.Length; i < l; i++)
                    {
                        string val = labels[i].Value;
                        // если нет перевода подставляю предупреждение с лейблом
                        if (string.IsNullOrEmpty(val)) val = "Empty translation for label: " + labels[i].Label;
                        errorMessage = errorMessage.Replace(labels[i].Label, val);

                        //errorMessage = errorMessage.Replace(labels[i].Label, labels[i].Value);
                    }

                    return errorMessage;
                }
            }
            catch (Exception) { }

            return text;
        }


        /// <summary>
        /// переводит текст с лейблами ##l@(.*?)@l##
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="text">json с множеством лейблов для перевода</param>
        /// <returns></returns>
        public string GetTranslation(string languageCode, string text, List<string> Paramenters = null)
        {
            if (!ExistsLabels(text)) return text;

            if (string.IsNullOrEmpty(languageCode)) languageCode = string.Empty;

            return GetTranslation(GetLanguageIdByCode(languageCode), text, Paramenters);
        }

        public string GetTranslation(string text, List<string> Paramenters = null)
        {
            var lang = GetUserLanguage();

            var res = GetTranslation(lang.Id, text);

            res = InsertValues(res, Paramenters);

            return res;

            //var currLang = GetLanguageCodeFromRequestUserLanguages(HttpContext.Current);
            //IContext defContext = null;

            //try
            //{
            //    defContext = DmsResolver.Current.Get<UserContexts>().Get(keepAlive: false, restoreToken: false);

            //    if (defContext.User.LanguageId <= 0) defContext = null;
            //}
            //catch
            //{ }

            //return defContext == null ? GetTranslation(currLang, text) : GetTranslation(defContext, text);
        }

        private string InsertValues(string Message, List<string> Paramenters)
        {
            var res = Message;

            if (Paramenters == null) return res;

            try
            {
                res = string.Format(Message, Paramenters.ToArray());
            }
            catch
            { }

            return res;
        }

        public int GetLanguageIdByCode(string languageCode)
        {
            // запрашиваю из кэша переводы
            var languageInfo = GetLanguageInfo();
            InternalAdminLanguage language = null;

            // languageCode может быть ru или ru_RU в зависимости от настроек браузера
            // нахожу локаль по имени 
            //var language = languageInfo.Languages.FirstOrDefault(x => languageCode.Equals(x.Code, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(languageCode))
            {
                language = languageInfo.Languages.FirstOrDefault(x => x.Code.Contains(languageCode));
            }

            // если локаль не определена, беру локаль по умолчанию
            if (language == null)
            {
                language = languageInfo.Languages.FirstOrDefault(x => x.IsDefault);
            }

            if (language == null) throw new DefaultLanguageIsNotSet();

            return language.Id;

        }

        public InternalAdminLanguage GetDefaultLanguage()
        {
            var languageInfo = GetLanguageInfo();
            var language = languageInfo.Languages.FirstOrDefault(x => x.IsDefault);
            return language;

        }

        public int GetLanguageIdByHttpContext()
        {
            var code = GetLanguageCodeFromRequestUserLanguages(HttpContext.Current);

            return GetLanguageIdByCode(code);
        }

        private InternalAdminLanguage GetUserLanguage()
        {
            var context = HttpContext.Current;

            var languageInfo = GetLanguageInfo();

            var userName = string.Empty;

            InternalAdminLanguage res = null;
            try
            {
                // тут я наблюдал, что Identity.Name возвращает имя последнего удачно авторизованного пользователя
                userName = HttpContext.Current.User.Identity.Name; //Request.GetOwinContext().Authentication.User.Identity.Name;// 

            }
            catch { }

            if (!string.IsNullOrEmpty(userName))
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();
                var languageId = webService.GetUserLanguageId(userName);

                res = languageInfo.Languages.FirstOrDefault(x => x.Id == languageId);
            }

            if (res == null)
            {
                var languageCode = string.Empty;

                try
                {
                    // получаю первый язык из массива языковых параметров клиента
                    // всегда пусто
                    languageCode = context.Request.UserLanguages?[0];

                    // Первый параметр может быть "ru-RU" или просто "ru"
                    if (!string.IsNullOrEmpty(languageCode)) languageCode = languageCode.Split('-')[0];
                }
                catch { }

                if (!string.IsNullOrEmpty(languageCode))
                {
                    res = languageInfo.Languages.FirstOrDefault(x => x.Code.Contains(languageCode));
                }
            }


            if (res == null)
            {
                // если локаль не определена, беру локаль по умолчанию
                res = languageInfo.Languages.FirstOrDefault(x => x.IsDefault);

                if (res == null) throw new DefaultLanguageIsNotSet();
            }

            return res;

        }

        private string GetLanguageCodeFromRequestUserLanguages(HttpContext context)
        {
            string languageName = string.Empty;

            try
            {
                // получаю первый язык из массива языковых параметров клиента
                // всегда пусто
                languageName = context.Request.UserLanguages?[0];

                if (!string.IsNullOrEmpty(languageName))
                // Первый параметр может быть "ru-RU" или просто "ru"
                { languageName = languageName.Split('-')[0]; }

                return languageName;
            }
            catch { }


            return languageName;

        }

        public string GetTranslation(IContext context, string text)
        {
            return GetTranslation(context.User.LanguageId, text);
        }

        public void RefreshLanguageValues()
        {
            // сбрасываю кэш
            _language = null;
            //DeleteAllAdminLanguageValues();
            //AddAdminLanguageValues(ApplicationDbImportData.GetAdminLanguageValues());
        }

        #endregion

        #region [+] DbProcess

        private AdminLanguageInfo GetAdminLanguageStruct(string serverPath)
        {
            var res = new AdminLanguageInfo();

            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                res.Languages = dbContext.AdminLanguagesSet.Select(x => new InternalAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    FileName = x.FileName,
                    IsDefault = x.IsDefault
                }).ToList();

                transaction.Complete();
            }

            foreach (var item in res.Languages)
            {

                var list = GetLanguageValues(item.FileName, serverPath).ToList();

                list.ForEach(x => x.LanguageId = item.Id);

                res.LanguageValues.AddRange(list);
            }

            return res;
        }

        public IEnumerable<InternalAdminLanguageValue> GetLanguageValues(string fileName, string serverPath)
        {

            var filePath = Path.Combine(serverPath, "App_Data", "LanguageValues", fileName);

            //------------------------------------------------------

            var list = new List<InternalAdminLanguageValue>();

            if (filePath.Trim() == string.Empty) return list;

            StreamReader reader;

            try
            {
                reader = File.OpenText(filePath);
            }
            catch (Exception)
            {

                return list;
            }

            string input;

            // построчно считываю файл с переводами.
            while ((input = reader.ReadLine()) != null)
            {
                if (input.Trim() == string.Empty) continue;

                var label = string.Empty;
                var value = string.Empty;

                try
                {
                    label = input.Split('=')[0].Trim();
                }
                catch { }

                try
                {
                    value = input.Split('=')[1].Trim();
                    //value = JsonConvert.SerializeObject(value, settings);
                }
                catch { }


                list.Add(new InternalAdminLanguageValue()
                {
                    Id = -1,
                    //LanguageId = languageId,
                    Label = label,
                    Value = value
                });
            }

            reader.Close();

            return list;
        }



        //private int AddAdminLanguageValue(InternalAdminLanguageValue model)
        //{
        //    using (var dbContext = new ApplicationDbContext())
        //    {
        //        var item = new AdminLanguageValues
        //        {
        //            LanguageId = model.LanguageId,
        //            Label = model.Label,
        //            Value = model.Value
        //        };
        //        dbContext.AdminLanguageValuesSet.Add(item);
        //        dbContext.SaveChanges();
        //        model.Id = item.Id;
        //        return item.Id;
        //    }
        //}


        //private void AddAdminLanguageValues(List<AdminLanguageValues> list)
        //{
        //    using (var dbContext = new ApplicationDbContext())
        //    {
        //        dbContext.AdminLanguageValuesSet.AddRange(list);
        //        dbContext.SaveChanges();
        //    }
        //}

        //private void UpdateAdminLanguageValue(InternalAdminLanguageValue model)
        //{
        //    using (var dbContext = new ApplicationDbContext())
        //    {
        //        var item = new AdminLanguageValues
        //        {
        //            Id = model.Id,
        //            LanguageId = model.LanguageId,
        //            Label = model.Label,
        //            Value = model.Value
        //        };
        //        dbContext.AdminLanguageValuesSet.Attach(item);
        //        var entity = dbContext.Entry(item);

        //        entity.Property(x => x.LanguageId).IsModified = true;
        //        entity.Property(x => x.Label).IsModified = true;
        //        entity.Property(x => x.Value).IsModified = true;
        //        dbContext.SaveChanges();
        //    }
        //}

        //private void DeleteAdminLanguageValue(InternalAdminLanguageValue model)
        //{
        //    using (var dbContext = new ApplicationDbContext())
        //    {
        //        dbContext.AdminLanguageValuesSet.RemoveRange(dbContext.AdminLanguageValuesSet.Where(x => x.Id == model.Id));
        //        dbContext.SaveChanges();
        //    }
        //}

        //private void DeleteAllAdminLanguageValues()
        //{
        //    using (var dbContext = new ApplicationDbContext())
        //    using (var transaction = Transactions.GetTransaction())
        //    {
        //        dbContext.AdminLanguageValuesSet.RemoveRange(dbContext.AdminLanguageValuesSet);
        //        dbContext.SaveChanges();
        //        transaction.Complete();
        //    }
        //}


        #endregion


    }
}