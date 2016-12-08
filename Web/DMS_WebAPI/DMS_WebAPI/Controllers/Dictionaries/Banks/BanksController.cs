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

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Банки (агенты)
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/Banks")]
    public class BanksController : ApiController
    {
        /// <summary>
        /// Возвращает список банков
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info")]
        [ResponseType(typeof(List<FrontDictionaryAgentBank>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentBank filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentBanks(ctx, filter, paging);
            var res= new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }
        /// <summary>
        /// Возвращает банк
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        // GET: api/DictionaryCompanies/5
        [HttpGet]
        [Route("Info/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentBank))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentBank(ctx, Id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавляет банк
        /// </summary>
        /// <param name="model">параметры юрлица</param>
        /// <returns>добавленную запись</returns>
        [HttpPost]
        [Route("Info")]
        public IHttpActionResult Post([FromBody]AddDictionaryAgentBank model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentBank, ctx, model));
        }

        ///// <summary>
        ///// добавить реквизиты банка к существующему контрагенту
        ///// </summary>
        ///// <param name="AgentId">ИД агента</param>
        ///// <param name="model">параметры банка</param>
        ///// <returns>добавленную запись</returns>
        //public IHttpActionResult PostToExistingAgent(int AgentId, [FromBody]ModifyDictionaryAgentBank model)
        //{
        //    var ctx = DmsResolver.Current.Get<UserContexts>().Get();
        //    var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
        //    model.Id = AgentId;
        //    return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentBank, ctx, model));
        //}
        /// <summary>
        /// Корректирует банковские реквизиты
        /// </summary>
        /// <param name="Id">ИД</param>
        /// <param name="model">параметры</param>
        /// <returns>возвращает измененную запись</returns>
        [HttpPut]
        [Route("Info")]
        public IHttpActionResult Put([FromBody]ModifyDictionaryAgentBank model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentBank, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет банк
        /// </summary>
        /// <param name="Id">ИД</param>
        /// <returns>ИД удаленной записи</returns>
        [HttpPost]
        [Route("Info/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentBank, ctx, Id);
            FrontDictionaryAgentBank tmp = new FrontDictionaryAgentBank();
            tmp.Id = Id;

            return new JsonResult(tmp, this);

        }

    }
}
