using DMS_WebAPI.Areas.HelpPage.ModelDescriptions;
using System.Collections.Generic;
using System.Linq;

namespace DMS_WebAPI.Areas.HelpPage.App_Start
{
    public static class Helper
    {
        public static string GetOrderedName(string Name) =>
            Name
            .Replace("FullTextSearchString", "AAA")
            .Replace("NotContainsIDs", "AAC")
            .Replace("IDs", "AAB")
            .Replace("Id", "AAB")
            .Replace("Code", "BBA")
            .Replace("FullName", "BBD")
            .Replace("FirstName", "BBC")
            .Replace("MiddleName", "BBC")
            .Replace("LastName", "BBC")
            .Replace("Name", "BBB")
            .Replace("CurrentPage", "ZZA")
            .Replace("PageSize", "ZZB")
            .Replace("IsAll", "ZZC")
            .Replace("IsOnlyCounter", "ZZD")
            .Replace("Sort", "ZZE");

        public static string GetParametrs(IList<ParameterDescription> parametrs, string dlm = ", ", bool InBrackets = true, bool MarkRequired = true)
        {
            var text = string.Empty;

            if (parametrs == null) return text;

            foreach (var item in parametrs
                            .OrderBy(x => x.Annotations.Any(y => y.Documentation == "Required") ? 0 : 1)
                            .ThenBy(x => GetOrderedName(x.Name)))
            {
                var itemName = item.Name;
                if (MarkRequired && item.Annotations.Any(x => x.Documentation == "Required"))
                {
                    itemName = "<b>" + itemName + "</b>";
                }
                text = text + (text == string.Empty ? string.Empty : dlm) + itemName;
            }

            if (InBrackets && !string.IsNullOrEmpty(text))
            {
                text = "(" + text + ")";
            }

            return text;
        }
    }
}