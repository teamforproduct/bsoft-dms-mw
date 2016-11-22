using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using DMS_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using BL.Model.SystemCore;

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
            // если нет кэша - первый вход и после сброса
            if (_language == null)
            {
                var nlst = GetAdminLanguageStruct();
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
                    var lst = GetAdminLanguageStruct();
                    _language.StoreObject = lst;
                    _language.LastUsage = DateTime.UtcNow;
                    return lst;
                }
                return _language.StoreObject as AdminLanguageInfo;
            }

        }

        public IEnumerable<InternalAdminLanguageValue> GetLanguageValues(FilterAdminLanguageValue filter)
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

            }

            return languageValues.ToList();
        }

        private bool ExistsLabels(string text)
        {
            return Regex.IsMatch(text, _PATTERN);
        }

        public string ReplaceLanguageLabel(int languageId, string text)
        {
            string errorMessage = text;

            if (languageId < 1)
            {
                // запрашиваю из кэша переводы
                var languageInfo = GetLanguageInfo();
                var language = languageInfo.Languages.FirstOrDefault(x => x.IsDefault);
                if (language != null) languageId = language.Id;
            }

            try
            {
                using (var dbContext = new ApplicationDbContext())
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
                    }

                    return errorMessage;
                }
            }
            catch (Exception ex) { }
            return text;
        }



        /// <summary>
        /// переводит текст с лейблами ##l@(.*?)@l##
        /// </summary>
        /// <param name="userLanguage"></param>
        /// <param name="text">json с множеством лейблов для перевода</param>
        /// <returns></returns>
        public string ReplaceLanguageLabel(string languageName, string text)
        {
            if (!ExistsLabels(text)) return text;

            // запрашиваю из кэша переводы
            var languageInfo = GetLanguageInfo();

            if (string.IsNullOrEmpty(languageName)) languageName = string.Empty;

            // нахожу локаль по имени 
            var language = languageInfo.Languages.FirstOrDefault(x => languageName.Equals(x.Code, StringComparison.OrdinalIgnoreCase));

            // если локаль не определена, беру локаль по умолчанию
            if (language == null)
            {
                language = languageInfo.Languages.FirstOrDefault(x => x.IsDefault);
            }

            return ReplaceLanguageLabel(language.Id, text);
        }

        public string ReplaceLanguageLabel(HttpContext Context, string text)
        {

            if (!ExistsLabels(text)) return text;

            string res = text;

            try
            {

                // pss Закоментировал. потому что ниже все равно еще раз будет перевод
                // сначала достаю перевод из DMS-Base
                //IContext ctx = null;
                //try
                //{
                //    ctx = DmsResolver.Current.Get<UserContext>().GetByLanguage();
                //    if (Context.User.Identity.IsAuthenticated && ctx != null)
                //    {
                //        var service = DmsResolver.Current.Get<ILanguageService>();
                //        //Перевод ошибки
                //        res = service.ReplaceLanguageLabel(ctx, res);
                //    }
                //}
                //catch { }

                // а потом еще раз достаю перевод из WEB-Base
                //var languageService = DmsResolver.Current.Get<Languages>();
                //Перевод ошибки на русский

                // получаю первый язык из массива языковых параметров клиента
                // всегда пусто
                string languageName = Context.Request.UserLanguages?[0];

                if (!string.IsNullOrEmpty(languageName))
                // Первый параметр может быть "ru-RU" или просто "ru"
                { languageName = languageName.Split('-')[0]; }

                res = ReplaceLanguageLabel(languageName, res);
            }
            catch { }

            return res;

            // этот код из JsonResult
            //try
            //{
            //    IContext ctx = null;
            //    try
            //    {
            //        ctx = DmsResolver.Current.Get<UserContext>().GetByLanguage();
            //        if (HttpContext.Current.User.Identity.IsAuthenticated && ctx != null)
            //        {
            //            var service = DmsResolver.Current.Get<ILanguageService>();
            //            json = service.ReplaceLanguageLabel(ctx, json);
            //        }
            //    }
            //    catch { }
            //    var languageService = DmsResolver.Current.Get<Languages>();
            //    json = languageService.ReplaceLanguageLabel(HttpContext.Current.Request.UserLanguages?[0], json);
            //}
            //catch { }

        }

        public string ReplaceLanguageLabel(IContext context, string text)
        {
            return ReplaceLanguageLabel(context.CurrentEmployee.LanguageId, text);
        }

        public void RefreshLanguageValues()
        {
            // сбрасываю кэш
            _language = null;
            //DeleteAllAdminLanguageValues();
            //AddAdminLanguageValues(ApplicationDbImportData.GetAdminLanguageValues());
        }

        public string GetTranslation(string text)
        {
            var httpContext = HttpContext.Current;
            IContext defContext = null;

            try
            {
                defContext = DmsResolver.Current.Get<UserContexts>().Get();

                if (defContext.CurrentEmployee.LanguageId <= 0) defContext = null;
            }
            catch
            { }

            if (defContext == null) return ReplaceLanguageLabel(httpContext, text);
            else return ReplaceLanguageLabel(defContext, text);
        }

        #endregion

        #region [+] DbProcess

        private AdminLanguageInfo GetAdminLanguageStruct()
        {
            var res = new AdminLanguageInfo();

            using (var dbContext = new ApplicationDbContext())
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                res.Languages = dbContext.AdminLanguagesSet.Select(x => new InternalAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).ToList();

                //res.LanguageValues = dbContext.AdminLanguageValuesSet.Select(x => new InternalAdminLanguageValue
                //{
                //    Id = x.Id,
                //    LanguageId = x.LanguageId,
                //    Label = x.Label,
                //    Value = x.Value
                //}).ToList();

                transaction.Complete();
            }

            foreach (var item in res.Languages)
            {
                
                var list = GetLanguageValues( item.Id);

                res.LanguageValues.AddRange(list);
            }

            return res;
        }

        public IEnumerable<InternalAdminLanguageValue> GetLanguageValues(int languageId)
        {

            // ХАЛЯВА
            var filePath = @"";
            switch (languageId)
            {
                case 45: //English
                    filePath += @"messages_en_US.properties";
                    break;
                case 740: //Polszczyzna
                    filePath += @"messages_pl_PL.properties";
                    break;
                case 90: //Беларуский
                    filePath += @"messages_be_BY.properties";
                    break;
                case 481: // Deutsch
                    filePath += @"messages_de_DE.properties";
                    break;
                case 745: //Francais
                    filePath += @"messages_fr_FR.properties";
                    break;
                case 570: //Русский
                    filePath += @"messages_ru_RU.properties";
                    break;
                case 720: //Українська
                    filePath += @"messages_uk_UA.properties";
                    break;
                case 790: //Cestina
                    filePath += @"messages_cs_CZ.properties";
                    break;
                default:
                    filePath += @"messages_ru_RU.properties";
                    break;
            }

            filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "App_Data", "LanguageValues", filePath);

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
                }
                catch { }


                list.Add(new InternalAdminLanguageValue()
                {
                    Id = -1,
                    LanguageId = languageId,
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
        //    using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
        //    {
        //        dbContext.AdminLanguageValuesSet.RemoveRange(dbContext.AdminLanguageValuesSet);
        //        dbContext.SaveChanges();
        //        transaction.Complete();
        //    }
        //}


        #endregion


    }
}