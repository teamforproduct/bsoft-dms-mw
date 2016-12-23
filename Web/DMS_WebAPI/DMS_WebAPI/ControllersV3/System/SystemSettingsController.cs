﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Системные настройки
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.System)]
    public class SystemSettingsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылка на все должности для исполнения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/IsSendAllForExecution")]
        public IHttpActionResult IsSendAllForExecution()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetSubordinationsSendAllForExecution(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// Если флаг TRUE, то при создании новой должности устанавливается рассылку на все должности для сведения, 
        /// в противном случае (FALSE) рассылка устанавливается в дефолт
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/IsSendAllForInforming")]
        public IHttpActionResult IsSendAllForInforming()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetSubordinationsSendAllForInforming(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/IsDigitalSignatureIsUseCertificateSign")]
        public IHttpActionResult IsDigitalSignatureIsUseCertificateSign()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetDigitalSignatureIsUseCertificateSign(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/IsDigitalSignatureIsUseInternalSign")]
        public IHttpActionResult IsDigitalSignatureIsUseInternalSign()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetDigitalSignatureIsUseInternalSign(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/FulltextRefreshTimeout")]
        public IHttpActionResult GetFulltextRefreshTimeout()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetFulltextRefreshTimeout(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/AutoplanTimeout")]
        public IHttpActionResult GetAutoplanTimeoutMinute()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetAutoplanTimeoutMinute(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/ClearTrashDocumentsTimeout")]
        public IHttpActionResult GetClearTrashDocumentsTimeoutMinute()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetClearTrashDocumentsTimeoutMinute(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Settings/ClearTrashDocumentsTimeoutMinuteForClear")]
        public IHttpActionResult GetClearTrashDocumentsTimeoutMinuteForClear()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetClearTrashDocumentsTimeoutMinuteForClear(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}