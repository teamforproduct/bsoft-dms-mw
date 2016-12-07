using System;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System.Web;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Text;
using BL.Logic.SystemServices.TempStorage;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Работа с общим представлением контрагента (Наименование, признак активности, список текущих типов)
    /// От списка текущих типов в интерфейсе отходы на другие контроллеры, возвращающие детальную информацию,
    /// соответствующую выбранному типу
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/DictionaryAgents")]
    public class DictionaryAgentsController : ApiController
    {
        /// <summary>
        /// Получение словаря контрагентов
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns>Список контрагентов
        /// </returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryAgent filter, [FromUri]UIPaging paging)
        {
            //TODO Краткий формат если фильтр не указан или содержит несколько типов
            //     Формат конкретного типа, если тип явно указан в фильтре

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetAgents(ctx, filter,paging);
            var res=new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Получение словаря агентов по ИД
        /// </summary>
        /// <param name="id">ИД агента</param>
        /// <returns>Агент</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetAgent(ctx, id);
            return new JsonResult(tmpDict, this);
        }
        /// <summary>
        /// Добавление контрагента
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgent model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgent, ctx, model));
        }
        /// <summary>
        /// Изменение контрагента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgent model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();

            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            
            if (model.ImageId.HasValue)
            {
                var tmpStore = DmsResolver.Current.Get<ITempStorageService>();
                var avaFile = tmpStore.ExtractStoreObject(model.ImageId.Value);
                if (avaFile is string)
                {
                    model.PostedFileData = avaFile as string;
                }
            }

            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgent, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление контрагента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgent, ctx, id);
            FrontDictionaryAgent tmp = new FrontDictionaryAgent();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }


        [HttpPost]
        [Route("SetImage")]
        public IHttpActionResult SetImage([FromUri]ModifyDictionaryAgentImage model)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];

            byte[] buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, file.ContentLength);
            var fileContent = Convert.ToBase64String(buffer);

            var tmpStore= DmsResolver.Current.Get<ITempStorageService>();
            var imgageId = tmpStore.AddToStore(EnumObjects.DictionaryAgents, model.AgentId, 0, fileContent);
            return new JsonResult(imgageId, this);
        }

        [HttpDelete]
        [Route("DeleteImage")]
        public IHttpActionResult DeleteImage([FromUri] int AgentId)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentImage, ctx, AgentId);
            return new JsonResult(null, this);
        }

        [HttpGet]
        [Route("GetImage")]
        [ResponseType(typeof(List<FrontDictionaryAgentUserPicture>))]
        public IHttpActionResult GetImage([FromUri] int AgentId)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryAgentUserPicture(ctx, AgentId);
            return new JsonResult(tmpItems, this);
        }

        [HttpGet]
        [Route("GetImageOnEdit")]
        [ResponseType(typeof(FrontDictionaryAgentUserPicture))]
        public IHttpActionResult GetImageOnEdit([FromUri] int ImageId)
        {
            var tmpStore = DmsResolver.Current.Get<ITempStorageService>();
            var img = tmpStore.GetStoreObject(ImageId) as string;

            var res = new FrontDictionaryAgentUserPicture
            {
                Id = 0,
                FileContent = img
            };

            return new JsonResult(res, this);
        }
    }
}