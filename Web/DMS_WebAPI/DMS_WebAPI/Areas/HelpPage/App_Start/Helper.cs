using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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


    }
}