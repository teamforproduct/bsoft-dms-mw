﻿using System.ComponentModel.DataAnnotations;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    /// <summary>
    /// Списки рассылок шаблонов документов
    /// </summary>
    [Authorize]
    public class TemplateDocumentsRestrictedSendListsController : ApiController
    {
        /// <summary>
        /// Получение всех ограничительных списков рассылок шаблона документов
        /// </summary>
        /// <returns>Список шаблонов документов</returns>
        public IHttpActionResult Get([Required]int templateId,[FromUri]FilterTemplateDocumentRestrictedSendList filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocumentRestrictedSendLists(cxt,templateId,filter);
            return new JsonResult(tmpDocs, this);
        }

        /// <summary>
        /// Получение ограничительного списка рассылок шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД списка рассылки</param>
        /// <returns>Шаблон документа</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDoc = tmpDocProc.GetTemplateDocumentRestrictedSendList(cxt, id);
            return new JsonResult(tmpDoc, this);
        }

        /// <summary>
        /// Добавление списка рассылки к шаблону документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocumentRestrictedSendLists model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.AddOrUpdateTemplateRestrictedSendList(cxt, model,EnumTemplateDocumentsActions.AddTemplateDocumentRestrictedSendList);
            return Get(tmpTemplate);
        }

        /// <summary>
        /// Изменение списка рассылки шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put([Required]int id, [FromBody]ModifyTemplateDocumentRestrictedSendLists model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.AddOrUpdateTemplateRestrictedSendList(cxt, model,EnumTemplateDocumentsActions.ModifyTemplateDocumentRestrictedSendList);
            return Get(tmpTemplate);
        }

        /// <summary>
        /// Удаление списка рассылки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            tmpDocProc.DeleteTemplateRestrictedSendList(cxt, id);

            var tmp = new FrontTemplateDocumentRestrictedSendLists {Id = id};

            return new JsonResult(tmp, this);

        }
    }
}
