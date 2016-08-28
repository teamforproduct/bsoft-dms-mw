using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.Actions;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Admins
{
    [Authorize]
    [RoutePrefix("api/v2/AdminActions")]
    public class AdminActionsController : ApiController
    {
        /// <summary>
        /// Список элементов меню, доступный пользователю
        /// </summary>
        /// <returns>Меню</returns>
        [Route("GetMainMenu")]
        [HttpPost]
        public IHttpActionResult GetMainMenu()
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminMainMenu(ctx);
            return new JsonResult(tmpItems, this);
        }

    }

    // !!! Добавить логику связ с перездом companies в структуру агентов

    // Добавить пользователя-сотрудника
    // Добавить администратора подразделения (AgentId, DepartmentId)
    // Удалить администратора подразделения (AgentId, DepartmentId)

    // Копирование настроек от одной должности к другой
    // Настройка включения ограничечений рассылки, св, исп, ж.р.

    // Весь список ролей с отмеченными существующими для должности (EditMode)


}
