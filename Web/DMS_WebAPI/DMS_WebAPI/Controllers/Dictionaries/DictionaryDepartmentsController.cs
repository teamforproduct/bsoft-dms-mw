using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Описывает отделы (подразделения) в компании.
    /// Отделы всегда подчинены компаниям, могут подчиняться вышестоящим отделам.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/DictionaryDepartments")]
    public class DictionaryDepartmentsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Структура предприятия"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Структура предприятия"</param>
        /// <returns>FrontDictionaryDictionaryDepartment</returns>
        [ResponseType(typeof(List<FrontDictionaryDepartment>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryDepartment filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDepartments(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Структура предприятия" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryDictionary</returns>
        [ResponseType(typeof(FrontDictionaryDepartment))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDepartment(ctx, id);
            return new JsonResult(tmpDict, this);
        }
        
        /// <summary>
        /// Возвращает префикс из котдов вышестоящих отделов для нового отдела 
        /// </summary>
        /// <param name="parentId">Вышестоящий отдел</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPrefix")]
        public IHttpActionResult GetPrefix(int parentId)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDepartmentPrefix(ctx, parentId);
            return new JsonResult(tmpDict, this);
        }


        /// <summary>
        /// Добавление записи в словаре "Структура предприятия"
        /// </summary>
        /// <param name="model">ModifyDictionaryDictionaryDepartment</param>
        /// <returns>Возвращает добавленную запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryDepartment model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddDepartment, ctx, model));
        }


        /// <summary>
        /// Изменение записи в словаре "Структура предприятия"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryDictionaryDepartment</param>
        /// <returns>Возвращает измененную запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryDepartment model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryDictionaryDepartment

            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyDepartment, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Структура предприятия"
        /// </summary>
        /// <returns>FrontDictionaryDepartment</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteDepartment, ctx, id);
            FrontDictionaryDepartment tmp = new FrontDictionaryDepartment();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}