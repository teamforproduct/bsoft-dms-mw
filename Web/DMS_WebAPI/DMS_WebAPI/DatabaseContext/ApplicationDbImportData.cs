﻿using BL.Model.Enums;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMS_WebAPI.Models
{
    public static class ApplicationDbImportData
    {

        private static int IdSequence = 0;

        #region [+] Languages ...

        public static List<AdminLanguages> GetAdminLanguages()
        {
            var items = new List<AdminLanguages>();

            items.Add(new AdminLanguages { Id = 570, Code = "ru", Name = "Русский", IsDefault = true });
            items.Add(new AdminLanguages { Id = 045, Code = "en", Name = "English", IsDefault = false });
            items.Add(new AdminLanguages { Id = 720, Code = "uk", Name = "Українська", IsDefault = false });
            items.Add(new AdminLanguages { Id = 090, Code = "be", Name = "Беларуский", IsDefault = false });
            items.Add(new AdminLanguages { Id = 790, Code = "cs", Name = "Čeština", IsDefault = false });
            items.Add(new AdminLanguages { Id = 481, Code = "de", Name = "Deutsch", IsDefault = false });
            items.Add(new AdminLanguages { Id = 745, Code = "fr", Name = "Français", IsDefault = false });
            items.Add(new AdminLanguages { Id = 740, Code = "pl", Name = "Polszczyzna", IsDefault = false });

            return items;
        }
        #endregion

        private static string GetLabel(string group, string itemName) => "##l@" + group.Trim() + ":" + itemName.Trim() + "@l##";



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
            var f = GetLabelsFromEnums();
            f.AddRange(GetLabelsFromExceptions());

            var lang = new Languages();

            var d = lang.GetLanguageValues(570).Select(x => x.Label).ToList();

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

            //foreach (var item in toRem.OrderBy(x => x))
            //{
            //    s += "- " + item + "\r\n";
            //}

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
