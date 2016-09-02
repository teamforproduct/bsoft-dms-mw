﻿using BL.Logic.DictionaryCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Common;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Действия связанные с пользовательской настройкой системы
    /// </summary>
    [Authorize]
    public class DictionaryStaffListController : ApiController
    {
        /// <summary>
        /// Возвращает штатное расписание. Компании -> Отделы -> Должности -> Исполнители
        /// </summary>
        /// <param name="filter">Фильтрация элементов по названию</param>
        /// <param name="startWith">Определяет с какого элемента построить дерево</param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri] FilterTree filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetStaffList(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

    }

}