
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;
using System.Web.Http.Description;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using System;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentsController : ApiController
    {
        /// <summary>
        /// Получение списка документов
        /// </summary>
        /// <param name="filter">модель фильтра документов</param>
        /// <param name="paging">paging</param>
        /// <returns></returns>
        [ResponseType(typeof(FrontDocument))]
        public IHttpActionResult Get([FromUri] FilterDocument filter, [FromUri]UIPaging paging)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docs = docProc.GetDocuments(ctx, filter, paging);
            timeDB.Stop();
            var res = new JsonResult(docs, this);
            res.Paging = paging;
            timeM.Stop();
            SaveToFile("DB: IDocumentService GetDocuments User: " + ctx.CurrentAgentId, timeDB.Elapsed.ToString("G"));
            SaveToFile("M: DocumentsController Get List User: " + ctx.CurrentAgentId, timeM.Elapsed.ToString("G"));
            return res;
        }

        private void SaveToFile(string method, string time)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath("~/SiteLog.txt"));
                try
                {
                    string line = $"{DateTime.Now.ToString("o")}\r\n method: {method}\r\n time:{time}";
                    sw.WriteLine(line);
                }
                catch
                {
                }
                finally
                {
                    sw.Close();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Получение документа по ИД
        /// </summary>
        /// <param name="id">ИД Документа</param>
        /// <param name="filter">Фильтр для получения документа по ИД</param>
        /// <returns>Документ</returns>
        public IHttpActionResult Get(int id, [FromUri]FilterDocumentById filter = null)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            var timeDB1 = new System.Diagnostics.Stopwatch();
            var timeDB2 = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            timeDB.Start();
            timeDB1.Start();
            var doc = docProc.GetDocument(ctx, id, filter);
            timeDB1.Stop();

            timeDB2.Start();
            var metaData = docProc.GetModifyMetaData(ctx, doc);
            timeDB2.Stop();
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("DB: IDocumentService GetDocument and GetModifyMetaData User: " + ctx.CurrentAgentId, timeDB.Elapsed.ToString("G"));
            SaveToFile("DB1: IDocumentService GetDocument User: " + ctx.CurrentAgentId, timeDB1.Elapsed.ToString("G"));
            SaveToFile("DB2: IDocumentService GetModifyMetaData User: " + ctx.CurrentAgentId, timeDB2.Elapsed.ToString("G"));
            SaveToFile("M: DocumentsController Get By Id User: " + ctx.CurrentAgentId, timeM.Elapsed.ToString("G"));

            return new JsonResult(doc, metaData, this);
        }

        /// <summary>
        /// Добавление документа по шаблону
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Добавленный документ</returns>
        public IHttpActionResult Post([FromBody]AddDocumentByTemplateDocument model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AddDocument, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("DB: IDocumentService AddDocumentByTemplateDocument User: " + ctx.CurrentAgentId, timeDB.Elapsed.ToString("G"));
            SaveToFile("M: DocumentsController Post User: " + ctx.CurrentAgentId, timeM.Elapsed.ToString("G"));
            return Get(docId);
            //return new JsonResult(null,this);
        }

        /// <summary>
        /// Модификация документа 
        /// </summary>
        /// <param name="model">Модель для обновления документа</param>
        /// <returns>Обновленный документ</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocument model)
        {
            model.Id = id;
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.ModifyDocument, ctx, model);
            timeDB.Stop();
            timeM.Stop();
            SaveToFile("DB: IDocumentService ModifyDocument User: " + ctx.CurrentAgentId, timeDB.Elapsed.ToString("G"));
            SaveToFile("M: DocumentsController Put User: " + ctx.CurrentAgentId, timeM.Elapsed.ToString("G"));


            return Get(docId);
            //return new JsonResult(null, this);
        }

        /// <summary>
        /// Удаление документа 
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocument, ctx, id);
            timeDB.Stop();
            timeM.Stop();
            SaveToFile("DB: IDocumentService DeleteDocument User: " + ctx.CurrentAgentId, timeDB.Elapsed.ToString("G"));
            SaveToFile("M: DocumentsController Delete  User: " + ctx.CurrentAgentId, timeM.Elapsed.ToString("G"));
            return new JsonResult(null, this);
        }

    }
}
