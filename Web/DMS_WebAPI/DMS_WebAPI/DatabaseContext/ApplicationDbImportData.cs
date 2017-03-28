using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Enums;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS_WebAPI.Models
{
    public static class ApplicationDbImportData
    {


        #region [+] Languages ...

        public static List<AdminLanguages> GetAdminLanguages()
        {
            var items = new List<AdminLanguages>();

            items.Add(new AdminLanguages { Id = 570, Code = "ru_RU", Name = "Русский", IsDefault = true });
            items.Add(new AdminLanguages { Id = 045, Code = "en_US", Name = "English", IsDefault = false });
            items.Add(new AdminLanguages { Id = 720, Code = "uk_UA", Name = "Українська", IsDefault = false });
            items.Add(new AdminLanguages { Id = 090, Code = "be_BY", Name = "Беларуский", IsDefault = false });
            items.Add(new AdminLanguages { Id = 790, Code = "cs_CZ", Name = "Čeština", IsDefault = false });
            items.Add(new AdminLanguages { Id = 481, Code = "de_DE", Name = "Deutsch", IsDefault = false });
            items.Add(new AdminLanguages { Id = 745, Code = "fr_FR", Name = "Français", IsDefault = false });
            items.Add(new AdminLanguages { Id = 740, Code = "pl_PL", Name = "Polszczyzna", IsDefault = false });

            return items;
        }
        #endregion

        private static string GetLabel(string group, string itemName) => "##l@" + group.Trim() + ":" + itemName.Trim() + "@l##";

        public static List<SystemControlQuestions> GetSystemControlQuestions()
        {
            var items = new List<SystemControlQuestions>();

            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.FavoriteMusician, Name = GetLabel("ControlQuestions", EnumControlQuestion.FavoriteMusician.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.StreetWhereGrewUp, Name = GetLabel("ControlQuestions", EnumControlQuestion.StreetWhereGrewUp.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.FavoriteHero, Name = GetLabel("ControlQuestions", EnumControlQuestion.FavoriteHero.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.GrandmaBirthday, Name = GetLabel("ControlQuestions", EnumControlQuestion.GrandmaBirthday.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.ParentsIndex, Name = GetLabel("ControlQuestions", EnumControlQuestion.ParentsIndex.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.FirstCar, Name = GetLabel("ControlQuestions", EnumControlQuestion.FirstCar.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.FavoriteTeacher, Name = GetLabel("ControlQuestions", EnumControlQuestion.FavoriteTeacher.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.FavoriteBook, Name = GetLabel("ControlQuestions", EnumControlQuestion.FavoriteBook.ToString()) });
            items.Add(new SystemControlQuestions { Id = (int)EnumControlQuestion.FavoriteGame, Name = GetLabel("ControlQuestions", EnumControlQuestion.FavoriteGame.ToString()) });

            return items;
        }

        public static List<AspNetLicences> GetAspNetLicences()
        {
            var items = new List<AspNetLicences>();

            //items.Add(new AspNetLicences { Id = 0, Name = "", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 1, Name = "Base licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 10, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 2, Name = "Small business licence", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = 50, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 3, Name = "Fixed Name business", Description = "", NamedNumberOfConnections = null, ConcurenteNumberOfConnections = null, DurationDay = 365, Functionals = null, IsActive = true });
            items.Add(new AspNetLicences { Id = 4, Name = "Unlimited", Description = "", NamedNumberOfConnections = 50, ConcurenteNumberOfConnections = null, DurationDay = null, Functionals = null, IsActive = true });

            return items;
        }

        public static void CheckLanguages()
        {
            //var  = GetLabelsFromEnums();

            var f = new List<string>();

            var tmpService = DmsResolver.Current.Get<ISystemService>();

            // Действия
            f.AddRange(tmpService.GetImportSystemActions().Select(x => x.Description));

            // Модули
            f.AddRange(tmpService.GetImportSystemModules().Select(x => x.Name));

            // Фичи
            f.AddRange(tmpService.GetImportSystemFeatures().Select(x => x.Name));

            // Исключения
            f.AddRange(GetLabelsFromExceptions());

            var languages = new Languages();

            var lang = languages.GetLanguages(new BL.Model.AdminCore.FilterModel.FilterAdminLanguage { IsDefault = true }).FirstOrDefault();

            if (lang == null) throw new Exception("Не установлен язык по умолчанию");

            var d = languages.GetLanguageValues(lang.FileName).Select(x => x.Label).ToList();

            var toAdd = new List<string>();
            var toRem = new List<string>();

            foreach (var item in f)
            {
                if (!ExistLabel(item, d))
                { toAdd.Add(item); }

            }

            foreach (var item in d)
            {
                if (!ExistLabel(item, f))
                { toRem.Add(item); }

            }

            var s = string.Empty;

            foreach (var item in toAdd.OrderBy(x => x))
            {
                s += item + "=\r\n";
            }

            if (!string.IsNullOrEmpty(s))
                throw new Exception(string.Format("Так не пойдет! Нужно поддерживать переводы в актуальном состоянии Add {0} Del {1} \r\n", toAdd.Count, toRem.Count) + s);


        }

        private static bool ExistLabel(string label, List<string> items)
        {
            foreach (var i in items)
            {
                if (i == label) return true;
            }

            return false;
        }

        private static List<string> GetLabelsFromEnums()
        {
            var list = new List<string>();

            list.AddRange(GetListByEnum<EnumAddressTypes>());
            list.AddRange(GetListByEnum<EnumContactTypes>());

            list.AddRange(GetListByEnum<EnumAccessLevels>());
            list.AddRange(GetListByEnum<EnumPositionExecutionTypes>());
            list.AddRange(GetListByEnum<EnumRegistrationJournalAccessTypes>());
            list.AddRange(GetListByEnum<EnumDocumentDirections>());
            list.AddRange(GetListByEnum<EnumDocumentTypes>());
            list.AddRange(GetListByEnum<EnumEventTypes>());
            list.AddRange(GetListByEnum<EnumImportanceEventTypes>());
            list.AddRange(GetListByEnum<EnumLinkTypes>());
            list.AddRange(GetListByEnum<EnumSendTypes>());
            list.AddRange(GetListByEnum<EnumStageTypes>());
            list.AddRange(GetListByEnum<EnumSubordinationTypes>());
            list.AddRange(GetListByEnum<EnumSubscriptionStates>());

            list.AddRange(GetListByEnum<EnumObjects>());
            list.AddRange(GetListByEnum<EnumSystemActions>());
            list.AddRange(GetListByEnum<EnumAdminActions>());
            list.AddRange(GetListByEnum<EnumDictionaryActions>());
            list.AddRange(GetListByEnum<EnumDocumentActions>());
            list.AddRange(GetListByEnum<EnumEncryptionActions>());
            list.AddRange(GetListByEnum<EnumPropertyActions>());
            list.AddRange(GetListByEnum<EnumSystemFormulas>());
            list.AddRange(GetListByEnum<EnumSystemPatterns>());
            list.AddRange(GetListByEnum<EnumSystemFormats>());
            return list;

        }

        private static List<string> GetLabelsFromExceptions()
        {
            var list = new List<string>();
            list.AddRange(GetListExeptions());
            return list;

        }

        private static List<string> GetListExeptions()
        {
            var qry = from t in System.Reflection.Assembly.GetAssembly(typeof(BL.Model.Exception.AccessIsDenied)).GetTypes()
                      where t.IsClass && t.Namespace == "BL.Model.Exception"
                      select GetLabel("DmsExceptions", t.Name);
            return qry.ToList();//.ForEach(t => Console.WriteLine(t.Name));
        }

        public static List<string> GetListByEnum<T>()
        {
            var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            var enumName = typeof(T).Name.Replace("Enum", "");
            return array
              .Select(a =>
                 GetLabel(enumName, a.ToString())
              )
              .ToList();
        }

    }
}
