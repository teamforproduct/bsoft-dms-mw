using BL.Logic.DictionaryCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Common;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.Tree;

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
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult Get([FromUri] FilterTree filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetStaffList(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        public IHttpActionResult Post()
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.AddStaffList(ctx);
            return new JsonResult(new { success = true, msg = "Еще 10 000 ведер и золотой ключик наш" } , this);
        }


        // Копирование настроек от одной должности к другой

        // Правила рассылки с учетом флага используется на предприятии или нет

        // Роли для должности (EditMode)

        // Роли для сотрудника (EditMode)
    }

}