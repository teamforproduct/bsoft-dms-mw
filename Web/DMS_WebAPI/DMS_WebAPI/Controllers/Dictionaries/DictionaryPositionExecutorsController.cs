using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Описывает сроки и тип исполнения должностных обязанностей.
    /// Одновременно на одну должность могут быть назначены несколько сотрудников. 
    /// Один - назначен штатно, один - исполняет обязанности, несколько реферируют.
    /// Кроме того, назначения могут находится в разных временных интервалах - известна история и план назначений.
    /// 
    /// В штатном расписании отображаются только те сотрудники, которые исполняют обязанности в данный момент по плану.
    /// Вся история назначений отображается в панели "Назначения"
    /// </summary>
    [Authorize]
    public class DictionaryPositionExecutorsController : ApiController
    {
        /// <summary>
        /// Возвращает исполнителей должности
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей "Исполнители должности"</param>
        /// <returns>FrontDictionaryPositionExecutors</returns>
        // GET: api/DictionaryPositionExecutors
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
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
        [ResponseType(typeof(FrontDictionaryPositionExecutor))]
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