﻿using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSubscriptionsController : ApiController
    {
        /// <summary>
        /// Получение списка подписей 
        /// </summary>
        /// <param name="filter">модель фильтра подписей</param>
        /// <param name="paging">paging</param>
        /// <returns>список подписей</returns>
        [ResponseType(typeof(FrontDocumentSubscription))]
        public IHttpActionResult Get([FromUri] FilterDocumentSubscription filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var subscriptions = docProc.GetDocumentSubscriptions(ctx, filter, paging);
            var res = new JsonResult(subscriptions, this);
            res.Paging = paging;
            return res;
        }
    }
}