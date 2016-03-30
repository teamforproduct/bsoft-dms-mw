using BL.Logic.DependencyInjection;
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetCustomDictionaryTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetCustomDictionaryType(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление словаря
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись словаря</returns>
        public IHttpActionResult Post([FromBody]ModifyCustomDictionaryType model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddCustomDictionaryType,  cxt, model));
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyCustomDictionaryType, cxt, model);
            return Get(model.Id);
        }

        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteCustomDictionaryType, cxt, id);
            return new JsonResult(null, this);
        }
    }
}