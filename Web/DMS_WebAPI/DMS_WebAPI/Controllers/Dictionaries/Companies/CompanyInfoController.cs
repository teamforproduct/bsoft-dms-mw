using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.SystemCore;
using System.Web.Http.Description;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Физические лица (агенты)
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "Companies")]
    public class CompanyInfoOldController : ApiController
    {

        /// <summary>
        /// Возвращает список физических лиц
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info")]
        [ResponseType(typeof(List<FrontDictionaryAgentPerson>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentPerson filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentPersons(ctx, filter, paging);
            var res = new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Возвращает физическое лицо
        /// </summary>
        /// <param name="id">ИД</param>
        /// <returns>запись справочника агентов-физлиц</returns>
        [HttpGet]
        [Route("Info/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentPerson))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetAgentPerson(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавляет физическое лицо
        /// </summary>
        /// <param name="model">параметры физлица</param>
        /// <returns>добавленную запись</returns>
        [HttpPost]
        [Route("Info")]
        public IHttpActionResult Post([FromBody]AddAgentPerson model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentPerson, ctx, model));
        }


        /// <summary>
        /// Корректирует физическое лицо.
        /// </summary>
        /// <param name="model">параметры</param>
        /// <returns>возвращает измененную запись</returns>
        [HttpPut]
        [Route("Info")]
        public IHttpActionResult Put([FromBody]ModifyAgentPerson model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentPerson, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет физлицо (агент удаляется, если он является только физлицом)
        /// </summary>
        /// <param name="Id">ИД</param>
        /// <returns>ИД удаленной записи</returns>
        [HttpDelete]
        [Route("Info/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentPerson, ctx, Id);


            FrontDictionaryAgentPerson tmp = new FrontDictionaryAgentPerson();
            tmp.Id = Id;

            return new JsonResult(tmp, this);

        }

    }
}