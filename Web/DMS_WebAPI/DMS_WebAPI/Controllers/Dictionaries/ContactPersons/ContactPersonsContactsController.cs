using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Контакты контактного лица
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "ContactPersons")]
    public class ContactPersonsContactsController : ApiController
    {

        /// <summary>
        /// Возвращает список контактов контактного лица
        /// </summary>
        /// <param name="PersonId">ИД агента</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Contacts")]
        [ResponseType(typeof(List<FrontDictionaryContact>))]
        public IHttpActionResult Get(int PersonId, [FromUri] FilterDictionaryContact filter)
        {
            if (filter == null) filter = new FilterDictionaryContact();

            if (filter.AgentIDs == null) filter.AgentIDs = new List<int> { PersonId };
            else filter.AgentIDs.Add(PersonId);

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryContacts(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает адрес по ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Contacts/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryContact))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryContact(ctx, Id);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Создает новый контакт контактного лица
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Contacts")]
        public IHttpActionResult Post([FromBody]AddDictionaryContact model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpService.ExecuteAction(EnumDictionaryActions.AddContactPersonContacts, ctx, model));
        }

        /// <summary>
        /// Корректирует контакт контактного лица
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Contacts")]
        public IHttpActionResult Put([FromBody]ModifyDictionaryContact model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyContactPersonContacts, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет контакт контактного лица
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Contacts/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeleteContactPersonContacts, ctx, Id);
            FrontDictionaryContact tmp = new FrontDictionaryContact();
            tmp.Id = Id;

            return new JsonResult(tmp, this);

        }
    }
}