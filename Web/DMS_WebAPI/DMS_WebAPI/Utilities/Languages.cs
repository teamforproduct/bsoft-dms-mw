using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Database;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Represent functionality to configure languages
    /// </summary>
    public class Languages : ILanguages
    {
        private const int _MINUTES_TO_UPDATE_INFO = 5;// int.MaxValue; 

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
                    LastUsage = DateTime.Now,
                    StoreObject = nlst
                };
                _language = nso;
                return nlst;
            }
            else
            {
                if ((DateTime.Now - _language.LastUsage).TotalMinutes > _MINUTES_TO_UPDATE_INFO)
                {
                    var lst = GetAdminLanguageStruct();
                    _language.StoreObject = lst;
                    _language.LastUsage = DateTime.Now;
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
            var languageValues = languageInfo.LanguageValues
                .Where(x => x.LanguageId == filter.LanguageId)
                .Where(x => filter.Labels.Contains(x.Label))
                .ToList();

            return languageValues;
        }

        public IEnumerable<InternalAdminLanguage> GetLanguages(FilterAdminLanguage filter)
        {
            // выгребаю все переводы из кэша 
            var languageInfo = GetLanguageInfo();

            // выгребаю переводы для заданных filter.Labels
            var languageValues = languageInfo.Languages
                .Where(x => filter.IDs.Contains(x.Id))
                .Where(x => x.Code == filter.Code)
                .ToList();

            return languageValues;
        }

        public string ReplaceLanguageLabel(int languageId, string text)
        {
            string errorMessage = text;

            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var labelsInText = new List<string>();
                    // нахожу в тексте все лейблы, которые нужно переводить
                    foreach (Match label in Regex.Matches(errorMessage, "##l@(.*?)@l##"))
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
            // запрашиваю из кэша переводы
            var languageInfo = GetLanguageInfo();

            if (string.IsNullOrEmpty(languageName)) languageName = string.Empty;

            // нахожу локаль по имени 
            // TODO PSS "ru-RU".Equals("ru", StringComparison.OrdinalIgnoreCase); возвращает false
            //var language = languageInfo.Languages.FirstOrDefault(x => languageName.Contains(x.Code));
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
            string res = text;

            // pss временно беру переводы из функции для инициализации переводов.
            // return ApplicationDbImportData.ReplaceLanguageLabel(Context.Request.UserLanguages?[0], res);

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

                res = ReplaceLanguageLabel(languageName, res);
            }
            catch { }

            return res;

            // этот индус-код из JsonResult
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
            DeleteAllAdminLanguageValues();
            AddAdminLanguageValues(ApplicationDbImportData.GetAdminLanguageValues());
        }

        #endregion

        #region [+] DbProcess

        private AdminLanguageInfo GetAdminLanguageStruct()
        {
            using (var dbContext = new ApplicationDbContext())
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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

                transaction.Complete();

                return res;
            }
        }

        private int AddAdminLanguageValue(InternalAdminLanguageValue model)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var item = new AdminLanguageValues
                {
                    LanguageId = model.LanguageId,
                    Label = model.Label,
                    Value = model.Value
                };
                dbContext.AdminLanguageValuesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }


        private void AddAdminLanguageValues(List<AdminLanguageValues> list)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.AdminLanguageValuesSet.AddRange(list);
                dbContext.SaveChanges();
            }
        }

        private void UpdateAdminLanguageValue(InternalAdminLanguageValue model)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var item = new AdminLanguageValues
                {
                    Id = model.Id,
                    LanguageId = model.LanguageId,
                    Label = model.Label,
                    Value = model.Value
                };
                dbContext.AdminLanguageValuesSet.Attach(item);
                var entity = dbContext.Entry(item);

                entity.Property(x => x.LanguageId).IsModified = true;
                entity.Property(x => x.Label).IsModified = true;
                entity.Property(x => x.Value).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        private void DeleteAdminLanguageValue(InternalAdminLanguageValue model)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.AdminLanguageValuesSet.RemoveRange(dbContext.AdminLanguageValuesSet.Where(x => x.Id == model.Id));
                dbContext.SaveChanges();
            }
        }

        private void DeleteAllAdminLanguageValues()
        {
            using (var dbContext = new ApplicationDbContext())
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                dbContext.AdminLanguageValuesSet.RemoveRange(dbContext.AdminLanguageValuesSet);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        #endregion


    }
}