using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using DMS_WebAPI.Utilities;
using System;
using System.Linq;
using System.Web;

namespace DMS_WebAPI
{
    public static class ApiPrefix
    {
        public const string V2 = "api/v2/";

        public const string V3 = "api/v3/";

        public static void VerifyPermission()
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            // это не оптимально
            var tmpItems = tmpService.GetUserPermissions(ctx);
            var p = GetPermission();
            if (p == null) throw new AccessIsDenied();
            // может сравнение без учета регистра
            var res = tmpItems.Any(x => 
            p.Module.Equals(x.Module, StringComparison.OrdinalIgnoreCase)
            && p.Feature.Equals(x.Feature, StringComparison.OrdinalIgnoreCase)
            && x.AccessType == p.Type.ToString());

            if (!res) throw new AccessIsDenied();
        }

        public static Permission GetPermission(EnumAccessTypes? AccessTypes = null)
        {
            var res = new Permission();
            var httpContext = HttpContext.Current;
            string url = string.Empty;

            try { url = httpContext.Request.Url.ToString(); } catch { }

            if (string.IsNullOrEmpty(url)) return null;

            //url = url.Replace(V3, string.Empty);
            string[] stringSeparators = new string[] { V3 };
            var arr = url.Split(stringSeparators, StringSplitOptions.None);
            arr = arr[1].Split('/');

            foreach (var item in arr)
            {

                // первый элемент
                if (string.IsNullOrEmpty(res.Module))
                {
                    res.Module = item.ToLower();
                    continue;
                }

                // На втором месте может быть Id
                int valueParsed;
                if (Int32.TryParse(item, out valueParsed)) continue;

                res.Feature = item.ToLower();
                break;
            }

            if (AccessTypes != null)
                res.Type = (EnumAccessTypes)AccessTypes;
            else
            {
                switch (httpContext.Request.HttpMethod.ToUpper())
                {
                    case "GET": res.Type = EnumAccessTypes.R; break;
                    case "POST": res.Type = EnumAccessTypes.C; break;
                    case "PUT": res.Type = EnumAccessTypes.U; break;
                    case "DELETE": res.Type = EnumAccessTypes.D; break;
                }
            }

            return res;
        }

        public class Permission
        {
            public string Module { get; set; }
            public string Feature { get; set; }
            public EnumAccessTypes Type { get; set; }
        }

    }
}