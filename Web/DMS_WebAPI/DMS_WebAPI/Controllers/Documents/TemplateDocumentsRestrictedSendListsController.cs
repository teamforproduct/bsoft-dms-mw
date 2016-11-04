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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocumentRestrictedSendLists(ctx,templateId,filter);
            return new JsonResult(tmpDocs, this);
        }

        /// <summary>
        /// Получение ограничительного списка рассылок шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД списка рассылки</param>
        /// <returns>Шаблон документа</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDoc = tmpDocProc.GetTemplateDocumentRestrictedSendList(ctx, id);
            return new JsonResult(tmpDoc, this);
        }

        /// <summary>
        /// Добавление списка рассылки к шаблону документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocumentRestrictedSendLists model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.AddTemplateDocumentRestrictedSendList,ctx,model);
            return Get((int)tmpTemplate);
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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.ModifyTemplateDocumentRestrictedSendList,ctx,model);
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

            tmpDocProc.ExecuteAction(EnumDocumentActions.DeleteTemplateDocumentRestrictedSendList,ctx, id);

            var tmp = new FrontTemplateDocumentRestrictedSendLists {Id = id};

            return new JsonResult(tmp, this);

        }
    }
}
