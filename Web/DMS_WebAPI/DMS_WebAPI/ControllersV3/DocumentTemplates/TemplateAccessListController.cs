﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore;
using BL.Model.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.DocumentTemplates
{
    /// <summary>
    /// Шаблоны документов. Ограничительные списки рассылки.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplateAccessListController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<ITemplateService>();
            var tmpItem = tmpService.GetTemplateRestrictedSendList(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список
        /// </summary>
        /// <param name="Id">Id шаблона</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.AccessList)]
        [ResponseType(typeof(List<FrontTemplateRestrictedSendList>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterTemplateRestrictedSendList filter)
        {return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            if (filter == null) filter = new FilterTemplateRestrictedSendList();
            filter.TemplateId =  Id ;

            var tmpService = DmsResolver.Current.Get<ITemplateService>();
            var tmpItems = tmpService.GetTemplateRestrictedSendLists(context, filter);
            var res = new JsonResult(tmpItems, this);
            return res;});
        }


        /// <summary>
        /// Возвращает по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AccessList + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateRestrictedSendList))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.AccessList)]
        public async Task<IHttpActionResult> Post([FromBody]AddTemplateRestrictedSendList model)
        {return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            var tmpItem = Action.ExecuteDocumentAction(context, EnumActions.AddTemplateRestrictedSendList, model);
            return GetById(context, tmpItem);});
        }

        /// <summary>
        /// Корректирует
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.AccessList)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyTemplateRestrictedSendList model)
        {return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            Action.ExecuteDocumentAction(context, EnumActions.ModifyTemplateRestrictedSendList, model);
            return GetById(context, model.Id);});
        }

        /// <summary>
        /// Удаляет
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.AccessList + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            Action.ExecuteDocumentAction(context, EnumActions.DeleteTemplateRestrictedSendList, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;});
        }

    }
}
