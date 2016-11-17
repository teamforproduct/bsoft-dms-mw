using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class CustomDictionaryTypesController : ApiController
    {
        /// <summary>
        /// Возвращает список словарей
        /// </summary>
        /// <param name="filter">Параметры для фильтрации списка словарей</param>
        /// <returns>Cписок словарей</returns>
        public IHttpActionResult Get([FromUri] FilterCustomDictionaryType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetCustomDictionaryTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetCustomDictionaryType(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление словаря
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись словаря</returns>
        public IHttpActionResult Post([FromBody]ModifyCustomDictionaryType model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddCustomDictionaryType,  ctx, model));
        }

        /// <summary>
        /// Изменение словаря
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Измененная запись словаря</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyCustomDictionaryType model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyCustomDictionaryType, ctx, model);
            return Get(model.Id);
        }

        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteCustomDictionaryType, ctx, id);
            return new JsonResult(null, this);
        }
    }
}