using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Подписи.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentSignController : ApiController
    {
        /// <summary>
        /// Возвращает список подписей
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Signs)]
        [ResponseType(typeof(List<FrontDocumentSubscription>))]
        public IHttpActionResult Get([FromUri] FilterDocumentSubscription filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocumentSubscriptions(ctx, filter, paging);
            var res = new JsonResult(items, this);
            res.Paging = paging;
            return res;
        }
        
        /// <summary>
        /// Регистрирует отклонение подписания
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectSigning")]
        [HttpPut]
        public IHttpActionResult RejectSigning(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.RejectSigning, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует отклонение визирования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectVisaing")]
        [HttpPut]
        public IHttpActionResult RejectVisaing(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.RejectVisaing, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует отклонение согласования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectАgreement")]
        [HttpPut]
        public IHttpActionResult RejectАgreement(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.RejectАgreement, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует отклонение утверждения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectАpproval")]
        [HttpPut]
        public IHttpActionResult RejectАpproval(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.RejectАpproval, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Регистрирует отозыв с подписания
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawSigning")]
        [HttpPut]
        public IHttpActionResult WithdrawSigning(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.WithdrawSigning, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует отозыв визирования
        ///  </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawVisaing")]
        [HttpPut]
        public IHttpActionResult WithdrawVisaing(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.WithdrawVisaing, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует отозыв согласования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawАgreement")]
        [HttpPut]
        public IHttpActionResult WithdrawАgreement(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.WithdrawАgreement, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует отозыв утверждения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawАpproval")]
        [HttpPut]
        public IHttpActionResult WithdrawАpproval(SendEventMessage model)
        {
            Action.Execute(EnumDocumentActions.WithdrawАpproval, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Регистрирует подписание
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixSigning")]
        [HttpPut]
        public IHttpActionResult AffixSigning(AffixSigning model)
        {
            Action.Execute(EnumDocumentActions.AffixSigning, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует визирование
        ///  </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixVisaing")]
        [HttpPut]
        public IHttpActionResult AffixVisaing(AffixSigning model)
        {
            Action.Execute(EnumDocumentActions.AffixVisaing, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует согласование
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixАgreement")]
        [HttpPut]
        public IHttpActionResult AffixАgreement(AffixSigning model)
        {
            Action.Execute(EnumDocumentActions.AffixАgreement, model);
            var res = new JsonResult(true, this);
            return res;
        }
        /// <summary>
        /// Регистрирует утверждение
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixАpproval")]
        [HttpPut]
        public IHttpActionResult AffixАpproval(AffixSigning model)
        {
            Action.Execute(EnumDocumentActions.AffixАpproval, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Добавлет подпись по собственной инициативе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/SelfAffixSigning")]
        [HttpPost]
        public IHttpActionResult SelfAffixSigning(SelfAffixSigning model)
        {
            Action.Execute(EnumDocumentActions.SelfAffixSigning, model);
            var res = new JsonResult(true, this);
            return res;
        }

        /// <summary>
        /// Проверяет подписи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DimanicAuthorize("R")]
        [Route(Features.Signs + "/VerifySigning")]
        [HttpPost]
        public IHttpActionResult VerifySigning([FromBody]Item model)
        {
            Action.Execute(EnumDocumentActions.VerifySigning, model.Id);
            var res = new JsonResult(null, this);
            return res;
        }

    }
}
