using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryAddressTypesController : ApiController
    {
        /// <summary>
        /// Возвращает список типов адресов
        /// </summary>
        /// <param name="filter">Параметры для фильтрации типов документа</param>
        /// <returns>Cписок типов адресов</returns>
        // GET: api/DictionaryAddressTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryAddressType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAddressTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает тип адреса по ID
        /// </summary>
        /// <returns>Тип адреса</returns>
        // GET: api/DictionaryAddressTypes/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAddressType(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи в справочник типы адресов
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAddressType model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAddressType, cxt, model));
        }

        /// <summary>
        /// Изменение записи справочника типы адресов 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа адреса</returns>
         public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAddressType model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAddressType, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>Возвращает id удаленной записи</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAddressType, cxt, id);
            FrontDictionaryAddressType tmp = new FrontDictionaryAddressType();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

    }
}