using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Должности.
    /// Должности всегда подчинены отделам.
    /// Значимость должносьти в отделе задается параметром Order
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Position)]
    public class PositionInfoController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPosition(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


        /// <summary>
        /// Возвращает список должностей. 
        /// </summary>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(List<FrontDictionaryPosition>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositions(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает должность по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryPosition))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет должность
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddPosition model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddPosition, model);
            return GetById(context, tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyPosition model)
        {
            Action.Execute(EnumDictionaryActions.ModifyPosition, model);
            return GetById(context, model.Id);
        }

        /// <summary>
        /// Удаляет должность
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeletePosition, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Изменяет порядок следования должности в отделе (нумерация с 1)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info + "/Order")]
        public IHttpActionResult SetOrder([FromBody]ModifyPositionOrder model)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            tmpItem.SetPositionOrder(cxt, model);
            var res = new JsonResult(model.Order, this);
            return res;
        }


    }
}
