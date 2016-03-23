using BL.Model.Database;
using BL.Model.Database.FrontModel;
using BL.Model.Database.IncomingModel;
using BL.Model.Exception;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Represent functionality to configure languages
    /// </summary>
    public class Languages
    {
        public string ReplaceLanguageLabel(string userLanguage, string text)
        {
            try {
                using (var dbContext = new ApplicationDbContext())
                {
                    if (string.IsNullOrEmpty(userLanguage)) userLanguage = string.Empty;
                    var labels = dbContext.AdminLanguagesSet
                        .Where(x => userLanguage.Equals(x.Code, StringComparison.OrdinalIgnoreCase) || x.IsDefault)
                        .OrderBy(x => x.IsDefault)
                        .Take(1)
                        .SelectMany(x => x.LanguageValues)
                        .ToArray();

                    //TODO оптимизировать
                    for (int i = 0, l = labels.Length; i < l; i++)
                    {
                        text = text.Replace(labels[i].Label, labels[i].Value);
                    }
                    return text;
                }
            }
            catch(Exception ex) { }
            return text;
        }
    }
}