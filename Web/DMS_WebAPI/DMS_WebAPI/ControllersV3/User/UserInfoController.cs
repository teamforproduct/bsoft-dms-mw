using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel.Employees;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.Users;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BL.Model.WebAPI.FrontModel;
using BL.Model.Exception;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Информация о себе всегда доступна
    /// Контекст пользователя (Все пользователя являются сотрудниками, но у сотрудника может быть выключена возможность авторизации)
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserInfoController : WebApiController
    {

        private ApplicationUserManager _userManager;

        public UserInfoController()
        {
        }

        public UserInfoController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }


        private IHttpActionResult GetById(IContext context)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var tmpItem = webService.GetUserInfo(context);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает информацию о пользователе: имя, логин, язык, табельный номер, инн, дата рождения, пол
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(FrontAgentEmployeeUser))]
        public IHttpActionResult Get()
        {
            //!ASYNC
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            return GetById(context);
        }

        /// <summary>
        /// Корректирует реквизиты пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyAgentUser model)
        {
            //!ASYNC
            var contexts = DmsResolver.Current.Get<UserContexts>();
            var context = contexts.Get();
            var webSeevice = DmsResolver.Current.Get<WebAPIService>();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            var employee = tmpService.GetDictionaryAgentEmployee(context, context.CurrentAgentId);

            employee.ImageId = model.ImageId;
            //employee.LanguageId = model.LanguageId;

            employee.FirstName = model.FirstName;
            employee.MiddleName = model.MiddleName;
            employee.LastName = model.LastName;
            employee.TaxCode = model.TaxCode;
            employee.IsMale = model.IsMale;
            employee.BirthDate = model.BirthDate;

            webSeevice.UpdateUserEmployee(context, employee);

            webSeevice.SetUserLanguage(context.User.Id, model.LanguageId);

            return GetById(context);
        }

        /// <summary>
        /// Возвращает набор прав в терминах: module, feature, CRUD
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Permissions)]
        [ResponseType(typeof(List<FrontPermission>))]
        public async Task<IHttpActionResult> GetPermissions()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IAdminService>();
                var tmpItem = tmpService.GetUserPermissions(context);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает историю подключений сотрудника
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AuthLog)]
        [ResponseType(typeof(List<FrontSystemSession>))]
        public async Task<IHttpActionResult> Get([FromUri]FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var sessions = ctxs.GetContextListQuery();

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var pSessions = (IQueryable<FrontSystemSession>)param;
                var tmpService = DmsResolver.Current.Get<ILogger>();
                if (filter == null) filter = new FilterSystemSession();
                filter.ExecutorAgentIDs = new List<int> { context.CurrentAgentId };
                var tmpItems = tmpService.GetSystemSessions(context, pSessions, filter, paging);
                var res = new JsonResult(tmpItems, this);
                res.Paging = paging;
                return res;
            }, sessions);
        }

        /// <summary>
        /// Устанавливает новый логин
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ChangeLogin)]
        public async Task<IHttpActionResult> ChangeLogin([FromBody]ChangeLogin model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Устанавливает локаль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Language")]
        public IHttpActionResult SetLanguage(SetUserLanguage model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.SetUserLanguage(User.Identity.GetUserId(), model.LanguageCode);

            return new JsonResult(null, this);
        }

       

        /// <summary>
        /// Устанавливает новый пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetUserPassword model)
        { 
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.SetUserPasswordAsync(User.Identity.GetUserId(), model);

            return new JsonResult(null, this);
        }

        /// <summary>
        /// Изменяет текущий пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ChangePassword)]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ChangeUserPassword model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ChangeUserPasswordAsync(User.Identity.GetUserId(), model);

            return new JsonResult(null, this);
        }

        /// <summary>
        /// Выход
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            DmsResolver.Current.Get<UserContexts>().Remove();

            Request.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            // JsonResult - восстанавливает контекст
            //return new JsonResult(null, this);
            return Ok();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

    }
}