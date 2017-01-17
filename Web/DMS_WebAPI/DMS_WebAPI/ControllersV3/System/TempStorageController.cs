using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.Common;
using System.Diagnostics;
using BL.Logic.SystemServices.TempStorage;
using System.Web;
using System;
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Временное хранилище файлов.
    /// Файлы, прикрепленные к объекту, сразу загружаются во временное файловое хранилище. При записи объекта с фронта отправляются Id файлов, которые из временного хранилища записываются вместе с объектом.   
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.System)]
    public class TempFileStorageController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

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
            if (!stopWatch.IsRunning) stopWatch.Restart();


            var tmpService = DmsResolver.Current.Get<ITempStorageService>();
            var img = tmpService.GetStoreObject(Id) as string;

            var tmpItem = new FrontFile
            {
                Id = Id,
                FileContent = img
            };

            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет файл во временное хранилище
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.TempFileStorage)]
        public IHttpActionResult Post()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
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
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpService = DmsResolver.Current.Get<ITempStorageService>();
            tmpService.ExtractStoreObject(Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}