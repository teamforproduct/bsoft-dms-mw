using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Enums;

namespace DMS_WebAPI
{
    public static class Action
    {
        //TODO ASYNC REMOVE CONTEXT FROM HERE
        public static int Execute(IContext context, EnumDictionaryActions action, object model)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var res = tmpService.ExecuteAction(action, context, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(IContext context, EnumAdminActions action, object model)
        {
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var res = tmpService.ExecuteAction(action, context, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(IContext context, EnumSystemActions action, object model)
        {
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            var res = tmpService.ExecuteAction(action, context, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(IContext context, EnumEncryptionActions action, object model)
        {
            var tmpService = DmsResolver.Current.Get<IEncryptionService>();
            var res = tmpService.ExecuteAction(action, context, model);

            return res is int ? (int)res : -1;
        }

        public static int Execute(IContext context, EnumDocumentActions action, object model, int? сurrentPositionId = null)
        {
            var tmpService = DmsResolver.Current.Get<IDocumentService>();
            var res = tmpService.ExecuteAction(action, context, model);

            return res is int ? (int)res : -1;
        }

    }
}