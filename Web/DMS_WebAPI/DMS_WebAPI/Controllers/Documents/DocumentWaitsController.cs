﻿using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using BL.Model.DocumentCore.IncomingModel;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DocumentWaits")]
    public class DocumentWaitsController : ApiController
    {
        /// <summary>
        /// Получение списка ожиданий use V3
        /// </summary>
        /// <param name="filter">модель фильтра ожиданий</param>
        /// <param name="paging">paging</param>
        /// <returns>список ожиданий</returns>
        [ResponseType(typeof(List<FrontDocumentWait>))]
        public IHttpActionResult Get([FromUri] FilterBase filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var waits = docProc.GetDocumentWaits(ctx, filter, paging);
            var res = new JsonResult(waits, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Получение списка ожиданий use V3
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns>Список ожиданий</returns>
        [HttpPost]
        [Route("GetList")]
        [ResponseType(typeof(List<FrontDocumentWait>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Paging == null) model.Paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var waits = docProc.GetDocumentWaits(ctx, model.Filter, model.Paging);
            var res = new JsonResult(waits, this);
            res.Paging = model.Paging;
            return res;
        }
    }
}