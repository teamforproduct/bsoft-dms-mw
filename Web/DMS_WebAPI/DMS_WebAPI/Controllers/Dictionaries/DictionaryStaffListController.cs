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
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Действия связанные с пользовательской настройкой системы
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/DictionaryStaffList")]
    public class DictionaryStaffListController : ApiController
    {
        /// <summary>
        /// Возвращает штатное расписание. Компании -> Отделы -> Должности -> Исполнители
        /// </summary>
        /// <param name="filter">Фильтрация элементов по названию</param>
        /// <param name="startWith">Определяет с какого элемента построить дерево</param>
        /// <returns></returns>
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryStaffList filter)
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


        //[HttpGet]
        //[Route("GetPositionRoles")]
        //public IHttpActionResult GetPositionRoles(int positionId, FilterAdminRole filter)
        //{
        //    if (filter == null) filter = new FilterAdminRole();

        //    if (filter.PositionIDs == null)
        //    { filter.PositionIDs = new List<int> { positionId }; }
        //    else
        //    { filter.PositionIDs.Add(positionId); }

        //    var ctx = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    var tmpItems = tmpService.GetPositionRoles(ctx, filter);
        //    return new JsonResult(tmpItems, this);
        //}

        //[HttpGet]
        //[Route("GetPositionRolesEditMode")]
        //public IHttpActionResult GetPositionRolesEditMode(int positionId, FilterAdminRole filter)
        //{
        //    if (filter == null) filter = new FilterAdminRole();

        //    if (filter.PositionIDs == null)
        //    { filter.PositionIDs = new List<int> { positionId }; }
        //    else
        //    { filter.PositionIDs.Add(positionId); }

        //    var ctx = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    var tmpItems = tmpService.GetPositionRolesEditMode(ctx, filter);
        //    return new JsonResult(tmpItems, this);
        //}

        // Копирование настроек от одной должности к другой

        // Правила рассылки с учетом флага используется на предприятии или нет

        // Роли для должности (EditMode)

        // Роли для сотрудника (EditMode)
    }

}