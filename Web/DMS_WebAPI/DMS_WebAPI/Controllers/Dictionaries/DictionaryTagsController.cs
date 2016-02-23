﻿using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.DictionaryCore.FilterModel;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryTagsController : ApiController
    {
        // GET: api/DictionaryTags
        /// <summary>
        /// Получить список доступных тегов для выставленых должностей
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список доступных тегов для выставленых должностей</returns>
        public IHttpActionResult Get([FromUri]FilterDictionaryTag filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryTags(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryTags/5
        /// <summary>
        /// Получить тег для выставленых должностей
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Тег. Если тег не найден или недоступен для выставленных должностей вернеться ошибка</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryTag(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление тега для конкретной позиции
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Тег</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryTag model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDictProc.ExecuteAction(EnumDictionaryAction.AddTag, cxt, model));
        }

        /// <summary>
        /// Модификация тега для конкретной позиции.
        /// В списке выбранных должностей должна быть должность такая же как у изменяемого тега
        /// CurrentPosition - не нужно
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Тег</returns>
        /// 
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryTag model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            tmpDictProc.ExecuteAction(EnumDictionaryAction.ModifyTag, cxt, model);
            return Get(model.Id);
        }
    }
}