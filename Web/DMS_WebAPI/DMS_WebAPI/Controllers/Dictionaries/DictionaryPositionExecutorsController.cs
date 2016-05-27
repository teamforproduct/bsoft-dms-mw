using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryPositionExecutorsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Исполнители Исполнители должности"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Исполнители должности"</param>
        /// <returns>FrontDictionaryPositionExecutors</returns>
        // GET: api/DictionaryPositionExecutors
        public IHttpActionResult Get([FromUri] FilterDictionaryPositionExecutor filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryPositionExecutors(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Исполнители должности" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryPositionExecutors</returns>
        // GET: api/DictionaryPositionExecutors/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryPositionExecutor(cxt, id);
            return new JsonResult(tmpDict, this);
        }


        /// <summary>
        /// Добавление записи в словаре "Исполнители должности"
        /// </summary>
        /// <param name="model">ModifyDictionaryPositionExecutor</param>
        /// <returns>DictionaryPositionExecutors</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryPositionExecutor model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddExecutor, cxt, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Исполнители должности"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryPositionExecutor</param>
        /// <returns>DictionaryPositionExecutors</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryPositionExecutor model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryPositionExecutor

            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyExecutor, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Исполнители должности"
        /// </summary>
        /// <returns>DictionaryPositionExecutors</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteExecutor, cxt, id);
            FrontDictionaryPositionExecutor tmp = new FrontDictionaryPositionExecutor();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}