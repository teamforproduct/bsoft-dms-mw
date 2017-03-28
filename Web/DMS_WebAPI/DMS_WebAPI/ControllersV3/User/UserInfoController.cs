using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.Users;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Models;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Информация о себе всегда доступна
    /// Контекст пользователя (Все пользователя являются сотрудниками, но у сотрудника может быть выключена возможность авторизации)
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserInfoController : ApiController
    {

        private const string LocalLoginProvider = "Local";
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
        public async Task<IHttpActionResult> Get()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context);
            });
        }

        /// <summary>
        /// Корректирует реквизиты пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyAgentUser model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var contexts = DmsResolver.Current.Get<UserContexts>();
                var webSeevice = DmsResolver.Current.Get<WebAPIService>();
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();

                var employee = tmpService.GetDictionaryAgentEmployee(context, context.CurrentAgentId);

                employee.ImageId = model.ImageId;
                employee.LanguageId = model.LanguageId;

                employee.Name = model.Name;
                employee.FirstName = model.FirstName;
                employee.MiddleName = model.MiddleName;
                employee.LastName = model.LastName;
                employee.TaxCode = model.TaxCode;
                employee.IsMale = model.IsMale;
                employee.BirthDate = model.BirthDate;

                webSeevice.UpdateUserEmployee(context, employee);

                contexts.UpdateLanguageId(employee.Id, model.LanguageId);

                return GetById(context);
            });
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
            return await this.SafeExecuteAsync(ModelState, context =>
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
            //TODO ASYNC
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var sesions = ctxs.GetContextListQuery();

            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<ILogger>();
                if (filter == null) filter = new FilterSystemSession();
                filter.ExecutorAgentIDs = new List<int> { context.CurrentAgentId };
                var tmpItems = tmpService.GetSystemSessions(context, sesions, filter, paging);
                var res = new JsonResult(tmpItems, this);
                res.Paging = paging;
                return res;
            });
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
        public async Task<IHttpActionResult> SetLanguage(SetUserLanguage model)
        {
            //TODO ASYNC
            var contexts = DmsResolver.Current.Get<UserContexts>();

            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItem = tmpService.SetAgentUserLanguage(context, model.LanguageCode);
                contexts.UpdateLanguageId(context.CurrentAgentId, tmpItem);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Устанавливает новый пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            //TODO ASYNC DONE
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
            }

            return Ok();
        }

        /// <summary>
        /// Изменяет текущий пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ChangePassword)]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ChangePasswordBindingModel model)
        {
            //TODO ASYNC DONE
            if (!ModelState.IsValid)
            {
                return new JsonResult(ModelState, false, this);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
            }

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindByIdAsync(User.Identity.GetUserId());

            user.IsChangePasswordRequired = false;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
            }

            var user_context = DmsResolver.Current.Get<UserContexts>();
            user_context.UpdateChangePasswordRequired(user.Id, false);

            return new JsonResult(null, this);
        }

        /// <summary>
        /// Выход
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public async Task<IHttpActionResult> Logout()
        {
            //TODO ASYNC

            DmsResolver.Current.Get<UserContexts>().Remove();

            Request.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return new JsonResult(null, this);
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