﻿using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Lists
{
    /// <summary>
    /// Системные справочники и типы.
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class SystemListsController : WebApiController
    {

        /// <summary>
        /// Действия над объектами системы
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Actions)]
        [ResponseType(typeof(List<FrontSystemAction>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterSystemAction filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                var tmpItems = tmpService.GetSystemActions(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Уровни доступа
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AccessLevels)]
        [ResponseType(typeof(List<FrontAdminAccessLevel>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterAdminAccessLevel filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetAdminAccessLevels(context, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }


        /// <summary>
        /// Типы назначений на должность (назначен, исполняет обязанности, референт)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AssignmentTypes)]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutorType>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryPositionExecutorType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryPositionExecutorTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Направление документа (входящий, исходящий, внутренний)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.DocumentDirections)]
        [ResponseType(typeof(List<FrontDictionaryDocumentDirection>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryDocumentDirection filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryDocumentDirections(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Типы событий с документами (добавлен файл, взят на контроль...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.EventTypes)]
        [ResponseType(typeof(List<FrontDictionaryEventType>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryEventType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryEventTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Форматы значений для формул
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Formats)]
        [ResponseType(typeof(List<FrontSystemFormat>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterSystemFormat filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                var tmpItems = tmpService.GetSystemFormats(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Функции для формул
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Formulas)]
        [ResponseType(typeof(List<FrontSystemFormula>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterSystemFormula filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                var tmpItems = tmpService.GetSystemFormulas(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Типы важных событий с документами (важные события, второстепенные события, факты движения документов, собственные примечания...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ImportanceEventTypes)]
        [ResponseType(typeof(List<FrontDictionaryImportanceEventType>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryImportanceEventType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryImportanceEventTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Локали (языки системы)
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route(Features.Languages)]
        [ResponseType(typeof(List<InternalAdminLanguage>))]
        public IHttpActionResult GetLanguages([FromUri] FilterAdminLanguage filter)
        {
            var tmpService = DmsResolver.Current.Get<ILanguages>();
            var tmpItems = tmpService.GetLanguages(filter);
            return new JsonResult(tmpItems, this);
        }


        /// <summary>
        /// Типы связей документов (в ответ, во исполнение, в дополнение, повторно)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.LinkTypes)]
        [ResponseType(typeof(List<FrontDictionaryLinkType>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryLinkType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryLinkTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Объекты системы
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Objects)]
        [ResponseType(typeof(List<FrontSystemObject>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterSystemObject filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                var tmpItems = tmpService.GetSystemObjects(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Элементы для формул
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Patterns)]
        [ResponseType(typeof(List<FrontSystemPattern>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterSystemPattern filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                var tmpItems = tmpService.GetSystemPatterns(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Результаты исполнения (изменение контроля, отзыв, отказ...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ResultTypes)]
        [ResponseType(typeof(List<FrontDictionaryResultType>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryResultType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryResultTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Еще одни типы рассылки (для рассмотрения, для исполнения, на контроль, на подпись, на визирование, на согласование, на утверждение...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SendTypes)]
        [ResponseType(typeof(List<FrontDictionarySendType>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionarySendType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionarySendTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }



        /// <summary>
        /// Тип этапа для разделения плана на подписи и рассылку
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route(Features.StageTypes)]
        [ResponseType(typeof(List<ListItem>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryStageType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryStageTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }

        /// <summary>
        /// Типы рассылки (для сведения, для исполнения)
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route(Features.SubordinationTypes)]
        [ResponseType(typeof(List<ListItem>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionarySubordinationType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionarySubordinationTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }


        /// <summary>
        /// Типы значений
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ValueTypes)]
        [ResponseType(typeof(List<FrontSystemValueType>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterSystemValueType filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ISystemService>();
                var tmpItems = tmpService.GetSystemValueTypes(context, filter);
                return new JsonResult(tmpItems, this);
            });
        }

    }
}