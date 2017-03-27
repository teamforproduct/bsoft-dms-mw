﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
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
    /// Типы документов
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.DocumentType)]
    public class DocumentTypeInfoController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryDocumentType(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Типы документов
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <param name="sorting">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontDictionaryDocumentType>))]
        public IHttpActionResult Get([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryDocumentType filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetMainDictionaryDocumentTypes(ctx, ftSearch, filter, paging, sorting);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Возвращает реквизиты типа документа
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryDocumentType))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет банк
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddDocumentType model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddDocumentType, model);
            return GetById(context, tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты банка
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyDocumentType model)
        {
            Action.Execute(EnumDictionaryActions.ModifyDocumentType, model);
            return GetById(context, model.Id);
        }

        /// <summary>
        /// Удаляет банк
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteDocumentType, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
