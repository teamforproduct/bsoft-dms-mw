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
    public class CustomDictionariesController : ApiController
    {
        /// <summary>
        /// Возвращает список значений словарей
        /// </summary>
        /// <param name="filter">Параметры для фильтрации списка значений словарей</param>
        /// <returns>Cписок словарей</returns>
        public IHttpActionResult Get([FromUri] FilterCustomDictionary filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetCustomDictionaries(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetCustomDictionary(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление значения словаря
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Записи словаря</returns>
        public IHttpActionResult Post([FromBody]ModifyCustomDictionary model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.AddCustomDictionary, cxt, model);
            return Get(model.DictionaryTypeId);
        }

        /// <summary>
        /// Изменение значения словаря
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Записи словаря</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyCustomDictionary model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyCustomDictionary, cxt, model);
            return Get(model.DictionaryTypeId);
        }

        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteCustomDictionary, cxt, id);
            return new JsonResult(null, this);
        }
    }
}