﻿using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryDepartmentsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Структура предприятия"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Структура предприятия"</param>
        /// <returns>FrontDictionaryDictionaryDepartment</returns>
        // GET: api/DictionaryDepartments
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
        /// <returns>FrontDictionaryDictionaryDepartment</returns>
        // GET: api/DictionaryDepartments/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDepartment(ctx, id);
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
        /// <returns>FrontDictionaryDictionaryDepartment</returns> 
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