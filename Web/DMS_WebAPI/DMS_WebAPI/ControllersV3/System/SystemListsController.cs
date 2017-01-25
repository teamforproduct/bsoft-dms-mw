using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// !!! Доступ не ограничен.
    /// Системные справочники и типы. Для вызова апи требуется авторизация (доступ для авторизованных пользователей не ограничивается) 
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class SystemListsController : ApiController
    {

        /// <summary>
        /// Действия над объектами системы
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Actions)]
        [ResponseType(typeof(List<FrontSystemAction>))]
        public IHttpActionResult Get([FromUri] FilterSystemAction filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemActions(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Уровни доступа
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AccessLevels)]
        [ResponseType(typeof(List<FrontAdminAccessLevel>))]
        public IHttpActionResult Get([FromUri] FilterAdminAccessLevel filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAdminAccessLevels(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Типы назначений на должность (назначен, исполняет обязанности, референт)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AssignmentTypes)]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutorType>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryPositionExecutorType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryPositionExecutorTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Направление документа (входящий, исходящий, внутренний)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.DocumentDirections)]
        [ResponseType(typeof(List<FrontDictionaryDocumentDirection>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentDirection filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentDirections(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Типы событий с документами (добавлен файл, взят на контроль...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.EventTypes)]
        [ResponseType(typeof(List<FrontDictionaryEventType>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryEventType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryEventTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Форматы значений для формул
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Formats)]
        [ResponseType(typeof(List<FrontSystemFormat>))]
        public IHttpActionResult Get([FromUri] FilterSystemFormat filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemFormats(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Функции для формул
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Formulas)]
        [ResponseType(typeof(List<FrontSystemFormula>))]
        public IHttpActionResult Get([FromUri] FilterSystemFormula filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemFormulas(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Типы важных событий с документами (важные события, второстепенные события, факты движения документов, собственные примечания...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ImportanceEventTypes)]
        [ResponseType(typeof(List<FrontDictionaryImportanceEventType>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryImportanceEventType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryImportanceEventTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
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
        public IHttpActionResult Get([FromUri] FilterDictionaryLinkType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryLinkTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Объекты системы
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Objects)]
        [ResponseType(typeof(List<FrontSystemObject>))]
        public IHttpActionResult Get([FromUri] FilterSystemObject filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemObjects(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Элементы для формул
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Patterns)]
        [ResponseType(typeof(List<FrontSystemPattern>))]
        public IHttpActionResult Get([FromUri] FilterSystemPattern filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemPatterns(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Результаты исполнения (изменение контроля, отзыв, отказ...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ResultTypes)]
        [ResponseType(typeof(List<FrontDictionaryResultType>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryResultType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryResultTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Еще одни типы рассылки (для рассмотрения, для исполнения, на контроль, на подпись, на визирование, на согласование, на утверждение...)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SendTypes)]
        [ResponseType(typeof(List<FrontDictionarySendType>))]
        public IHttpActionResult Get([FromUri] FilterDictionarySendType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionarySendTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Типы рассылки (для сведения, для исполнения)
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [HttpGet]
        [Route(Features.SubordinationTypes)]
        [ResponseType(typeof(List<FrontDictionarySubordinationType>))]
        public IHttpActionResult Get([FromUri] FilterDictionarySubordinationType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionarySubordinationTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }


        /// <summary>
        /// Типы значений
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ValueTypes)]
        [ResponseType(typeof(List<FrontSystemValueType>))]
        public IHttpActionResult Get([FromUri] FilterSystemValueType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpSysProc = DmsResolver.Current.Get<ISystemService>();
            var tmpDicts = tmpSysProc.GetSystemValueTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

    }
}