using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Enums;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI
{
    public static class Action
    {
        //TODO ASYNC REMOVE CONTEXT FROM HERE
        public static int Execute(EnumDictionaryActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(EnumAdminActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(EnumSystemActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(EnumEncryptionActions action, object model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(EnumDocumentActions action, object model, int? сurrentPositionId = null)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(сurrentPositionId);
            var tmpService = DmsResolver.Current.Get<IDocumentService>();
            var res = tmpService.ExecuteAction(action, ctx, model);

            return res is int ? (int)res : -1;
        }

    }
}