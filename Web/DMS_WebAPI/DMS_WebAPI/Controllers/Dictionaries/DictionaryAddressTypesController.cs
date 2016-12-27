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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAddressTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает тип адреса по ID
        /// </summary>
        /// <returns>Тип адреса</returns>
        // GET: api/DictionaryAddressTypes/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAddressType(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи в справочник типы адресов
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Post([FromBody]AddAddressType model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAddressType, ctx, model));
        }

        /// <summary>
        /// Изменение записи справочника типы адресов 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа адреса</returns>
         public IHttpActionResult Put(int id, [FromBody]ModifyAddressType model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAddressType, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>Возвращает id удаленной записи</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAddressType, ctx, id);
            FrontAddressType tmp = new FrontAddressType();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

    }
}