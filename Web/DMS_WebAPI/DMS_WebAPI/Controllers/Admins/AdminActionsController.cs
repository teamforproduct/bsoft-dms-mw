using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.Actions;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Enums;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Действия связанные с пользовательской настройкой системы
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/AdminActions")]
    public class AdminActionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        //[Route("AddNewClient")]
        //[HttpPost]
        //public IHttpActionResult AddNewClient([FromBody]AddClient model)
        //{
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddRole, cxt, model);
        //    return Get((int)tmpItem);
        //}

    }


    // Добавить пользователя-сотрудника
    // Добавить администратора подразделения (AgentId, DepartmentId)
    // Удалить администратора подразделения (AgentId, DepartmentId)

    // Копирование настроек от одной должности к другой
    // Настройка включения ограничечений рассылки, св, исп, ж.р.

    // Весь список ролей с отмеченными существующими для должности (EditMode)


}
