using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Enums;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_WebAPI
{
    public static class Action
    {
        public static int Execute(EnumDictionaryActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            if (res is int) return (int)res;
            else return -1;
        }

        public static int Execute(EnumAdminActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            if (res is int) return (int)res;
            else return -1;
        }

        public static int Execute(EnumSystemActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            if (res is int) return (int)res;
            else return -1;
        }

        public static int Execute(EnumEncryptionActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            if (res is int) return (int)res;
            else return -1;
        }
    }
}