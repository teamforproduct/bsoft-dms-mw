using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Временное хранилище файлов.
    /// Файлы, прикрепленные к объекту, сразу загружаются во временное файловое хранилище. При записи объекта с фронта отправляются Id файлов, которые из временного хранилища записываются вместе с объектом.   
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class TempFileStorageController : ApiController
    {
        /// <summary>
        /// Возвращает файл из временного хранилища
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.TempFileStorage + "/{Id:int}")]
        [ResponseType(typeof(FrontFile))]
        public IHttpActionResult Get(int Id)
        {


            var tmpService = DmsResolver.Current.Get<ITempStorageService>();
            var img = tmpService.GetStoreObject(Id) as string;

            var tmpItem = new FrontFile
            {
                Id = Id,
                FileContent = img
            };

            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет файл во временное хранилище
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.TempFileStorage)]
        public IHttpActionResult Post()
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];

            byte[] buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, file.ContentLength);
            var fileContent = Convert.ToBase64String(buffer);

            var tmpService = DmsResolver.Current.Get<ITempStorageService>();
            var imgageId = tmpService.AddToStore(EnumObjects.DictionaryAgents, -1, 0, fileContent);
            return new JsonResult(imgageId, this);
        }


        /// <summary>
        /// Удаляет файл из временного хранилища
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.TempFileStorage + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var tmpService = DmsResolver.Current.Get<ITempStorageService>();
            tmpService.ExtractStoreObject(Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
    }
}