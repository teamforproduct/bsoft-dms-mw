using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

using BL.Model.Common;
using System.Web;
using BL.Logic.SystemServices.TempStorage;

namespace DMS_WebAPI.Controllers.Dictionaries
{/// <summary>
 /// Контрагент - сотрудник
 /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/DictionaryAgentEmployees")]
    public class DictionaryAgentEmployeesController : ApiController
    {
        /// <summary>
        /// Список всех сотрудников
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<FrontDictionaryAgentEmployee>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryAgentEmployees(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Возвращает список (Id, Name) всех сотрудников
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetList")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentEmployeeList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Запись справочника сотрудников
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(FrontDictionaryAgentEmployee))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentEmployee(ctx, id);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Добавление сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]AddDictionaryAgentEmployee model)
        {
            var dbWebProc = new WebAPIDbProcess();

            var tmpItem = dbWebProc.AddUserEmployee(model);

            return Get(tmpItem);
        }

        /// <summary>
        /// сделать физлицо сотрудником
        /// </summary>
        /// <param name="AgentPersonId">ИД агента</param>
        /// <param name="model">параметры сотрудника</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult PostToExistingAgent(int AgentPersonId, [FromBody]ModifyDictionaryAgentEmployee model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            model.Id = AgentPersonId;
            return Get((int)tmpItem.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, ctx, model));
        }

        /// <summary>
        /// Изменение сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentEmployee model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();

            if (model.ImageId.HasValue)
            {
                var tmpStore = DmsResolver.Current.Get<ITempStorageService>();
                var avaFile = tmpStore.ExtractStoreObject(model.ImageId.Value);
                if (avaFile is HttpPostedFile)
                {
                    model.PostedFileData = avaFile as HttpPostedFile;
                }
            }

            tmpItem.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployee, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();

            tmpItem.ExecuteAction(EnumDictionaryActions.DeleteAgentEmployee, ctx, id);
            FrontDictionaryAgentPerson tmp = new FrontDictionaryAgentPerson();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

        /// <summary>
        /// Устанавливает язык интерфейса для пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("SetLanguage")]
        public IHttpActionResult SetLanguage(int id, int languageId)
        {
            var contexts = DmsResolver.Current.Get<UserContexts>();
            var ctx = contexts.Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            tmpItem.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployeeLanguage, ctx, new ModifyDictionaryAgentUser { Id = id, LanguageId = languageId });
            contexts.UpdateLanguageId(id, languageId);

            return new JsonResult(null, this);
        }

        /// <summary>
        /// Устанавливает настройки для пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("SetSettings")]
        public IHttpActionResult SetSettings(int id, ModifyDictionaryAgentUserSettings model)
        {
            var contexts = DmsResolver.Current.Get<UserContexts>();
            var ctx = contexts.Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
//            tmpItem.ExecuteAction(EnumDictionaryActions.ModifyAgentEmployeeLanguage, ctx, new ModifyDictionaryAgentUser { Id = id, LanguageId = languageId });

            return new JsonResult(null, this);
        }
    }
}
