using System.ComponentModel.DataAnnotations;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Web.Http.Description;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Documents
{
    /// <summary>
    /// Списки рассылок шаблонов документов
    /// </summary>
    [Authorize]
    public class TemplateDocumentsSendListsController : ApiController
    {
        /// <summary>
        /// Получение всех списков рассылок шаблона документов
        /// </summary>
        /// <returns>Список шаблонов документов</returns>
        [ResponseType(typeof(List<FrontTemplateDocumentSendList>))]
        public IHttpActionResult Get([FromUri]FilterTemplateDocumentSendList filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocumentSendLists(ctx,filter);
            return new JsonResult(tmpDocs, this);
        }

        /// <summary>
        /// Получение списка рассылок шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД списка рассылки</param>
        /// <returns>Шаблон документа</returns>
        [ResponseType(typeof(FrontTemplateDocumentSendList))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDoc = tmpDocProc.GetTemplateDocumentSendList(ctx, id);
            return new JsonResult(tmpDoc, this);
        }

        /// <summary>
        /// Добавление списка рассылки к шаблону документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocumentSendLists model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.AddTemplateDocumentSendList,ctx,model);
            return Get((int)tmpTemplate);
        }

        /// <summary>
        /// Изменение списка рассылки шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put([Required]int id, [FromBody]ModifyTemplateDocumentSendLists model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.ModifyTemplateDocumentSendList,ctx,model);
            return Get((int)tmpTemplate);
        }

        /// <summary>
        /// Удаление списка рассылки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            tmpDocProc.ExecuteAction(EnumDocumentActions.DeleteTemplateDocumentSendList,ctx, id);

            var tmp = new FrontTemplateDocumentSendList {Id = id};

            return new JsonResult(tmp, this);

        }
    }
}
