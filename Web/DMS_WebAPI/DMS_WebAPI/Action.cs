using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Logic.PropertyCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Enums;

namespace DMS_WebAPI
{
    public static class Action
    {
        //TODO ASYNC REMOVE CONTEXT FROM HERE
        public static int ExecuteEncryptionAction(IContext context, EnumActions action, object model)
        {
            var _service = DmsResolver.Current.Get<IEncryptionService>();
            var res = _service.ExecuteAction(action, context, model);
            return res is int ? (int)res : -1;
        }
        public static int ExecuteSystemAction(IContext context, EnumActions action, object model)
        {
            var _service = DmsResolver.Current.Get<ISystemService>();
            var res = _service.ExecuteAction(action, context, model);
            return res is int ? (int)res : -1;
        }
        public static int ExecutePropertyAction(IContext context, EnumActions action, object model)
        {
            var _service = DmsResolver.Current.Get<IPropertyService>();
            var res = _service.ExecuteAction(action, context, model);
            return res is int ? (int)res : -1;
        }
        public static int ExecuteAdminAction(IContext context, EnumActions action, object model)
        {
            var _service = DmsResolver.Current.Get<IAdminService>();
            var res = _service.ExecuteAction(action, context, model);
            return res is int ? (int)res : -1;
        }

        public static int ExecuteDictionaryAction(IContext context, EnumActions action, object model)
        {
            var _service = DmsResolver.Current.Get<IDictionaryService>();
            var res = _service.ExecuteAction(action, context, model);
            return res is int ? (int)res : -1;
        }

        public static int ExecuteDocumentAction(IContext context, EnumActions action, object model, int? сurrentPositionId = null)
        {
            if (сurrentPositionId.HasValue)
                context.SetCurrentPosition(сurrentPositionId);
            var _service = DmsResolver.Current.Get<IDocumentService>();
            var res = _service.ExecuteAction(action, context, model);
            return res is int ? (int)res : -1;
        }

    }
}