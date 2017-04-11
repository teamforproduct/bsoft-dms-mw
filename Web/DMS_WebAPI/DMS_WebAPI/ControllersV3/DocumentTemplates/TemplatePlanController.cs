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
    /// Шаблоны документов. Рассылка.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplatePlanController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateDocumentSendList(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список рассылок
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Plan)]
        [ResponseType(typeof(List<FrontTemplateDocumentSendList>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterTemplateDocumentSendList filter)
        {
            if (filter == null) filter = new FilterTemplateDocumentSendList();
            filter.TemplateId = Id;

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
                var tmpItems = tmpService.GetTemplateDocumentSendLists(context, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }


        /// <summary>
        /// Возвращает пункт списка рассылки по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Plan + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateDocumentSendList))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет пункт в список рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Plan)]
        public async Task<IHttpActionResult> Post([FromBody]AddTemplateDocumentSendList model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.AddTemplateDocumentSendList, model);
                   return GetById(context, tmpItem);
               });
        }

        /// <summary>
        /// Корректирует пункт в списке рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Plan)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyTemplateDocumentSendList model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.ModifyTemplateDocumentSendList, model);
                   return GetById(context, model.Id);
               });
        }

        /// <summary>
        /// Удаляет пункт из списка рассылки
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Plan + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.DeleteTemplateDocumentSendList, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

    }
}
