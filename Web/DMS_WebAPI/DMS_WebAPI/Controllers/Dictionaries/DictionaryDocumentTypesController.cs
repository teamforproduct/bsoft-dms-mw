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
    public class DictionaryDocumentTypesController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Типы документов"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Типы документов"</param>
        /// <returns>FrontDictionaryDocumentType</returns>
        // GET: api/DictionaryDocumentTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Типы документов" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryDocumentType</returns>
        // GET: api/DictionaryDocumentTypes/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDocumentType(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи в словаре "Типы документов"
        /// </summary>
        /// <param name="model">ModifyDictionaryDocumentType</param>
        /// <returns>Возвращает добавленную запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryDocumentType model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddDocumentType,  ctx, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Типы документов"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model">ModifyDictionaryDocumentType</param>
        /// <returns>Возвращает измененную запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryDocumentType model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyDocumentType, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Типы документов"
        /// </summary>
        /// <returns>Возвращает ID удаленного документа</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteDocumentType, ctx, id);
            FrontDictionaryDocumentType tmp = new FrontDictionaryDocumentType();
            tmp.Id = id;

            return new JsonResult(tmp, this);
            
        }

    }
}