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
using System.Threading.Tasks;
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
        public async Task<IHttpActionResult> Get([FromUri] FilterDocumentSubscription filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentService>();
                   var items = docProc.GetDocumentSubscriptions(context, filter, paging);
                   var res = new JsonResult(items, this);
                   res.Paging = paging;
                   return res;
               });
        }

        /// <summary>
        /// Регистрирует отклонение подписания
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectSigning")]
        [HttpPut]
        public async Task<IHttpActionResult> RejectSigning(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.RejectSigning, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует отклонение визирования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectVisaing")]
        [HttpPut]
        public async Task<IHttpActionResult> RejectVisaing(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.RejectVisaing, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует отклонение согласования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectАgreement")]
        [HttpPut]
        public async Task<IHttpActionResult> RejectАgreement(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.RejectАgreement, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует отклонение утверждения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/RejectАpproval")]
        [HttpPut]
        public async Task<IHttpActionResult> RejectАpproval(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.RejectАpproval, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }

        /// <summary>
        /// Регистрирует отозыв с подписания
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawSigning")]
        [HttpPut]
        public async Task<IHttpActionResult> WithdrawSigning(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.WithdrawSigning, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует отозыв визирования
        ///  </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawVisaing")]
        [HttpPut]
        public async Task<IHttpActionResult> WithdrawVisaing(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.WithdrawVisaing, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует отозыв согласования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawАgreement")]
        [HttpPut]
        public async Task<IHttpActionResult> WithdrawАgreement(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.WithdrawАgreement, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует отозыв утверждения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/WithdrawАpproval")]
        [HttpPut]
        public async Task<IHttpActionResult> WithdrawАpproval(SendEventMessage model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.WithdrawАpproval, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }

        /// <summary>
        /// Регистрирует подписание
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixSigning")]
        [HttpPut]
        public async Task<IHttpActionResult> AffixSigning(AffixSigning model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.AffixSigning, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует визирование
        ///  </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixVisaing")]
        [HttpPut]
        public async Task<IHttpActionResult> AffixVisaing(AffixSigning model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.AffixVisaing, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует согласование
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixАgreement")]
        [HttpPut]
        public async Task<IHttpActionResult> AffixАgreement(AffixSigning model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.AffixАgreement, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }
        /// <summary>
        /// Регистрирует утверждение
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/AffixАpproval")]
        [HttpPut]
        public async Task<IHttpActionResult> AffixАpproval(AffixSigning model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.AffixАpproval, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }

        /// <summary>
        /// Добавлет подпись по собственной инициативе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Signs + "/SelfAffixSigning")]
        [HttpPost]
        public async Task<IHttpActionResult> SelfAffixSigning(SelfAffixSigning model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.SelfAffixSigning, model);
                   var res = new JsonResult(true, this);
                   return res;
               });
        }

        /// <summary>
        /// Проверяет подписи
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DimanicAuthorize("R")]
        [Route(Features.Signs + "/VerifySigning")]
        [HttpPost]
        public async Task<IHttpActionResult> VerifySigning([FromBody]Item model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
               {
                   Action.Execute(context, EnumDocumentActions.VerifySigning, model.Id);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

    }
}
