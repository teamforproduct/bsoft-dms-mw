﻿using BL.CrossCutting.DependencyInjection;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.WebAPI
{
    [Authorize]
    public class ServersController : ApiController
    {
        /// <summary>
        /// Получение списка серверов
        /// </summary>
        /// <returns>список серверов</returns>
        public IHttpActionResult Get(FilterAdminServers filter)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var items = dbProc.GetServers(filter);
            return new JsonResult(items, this);
        }
        /// <summary>
        /// Получение сервера
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Get(int id)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var item = dbProc.GetServer(id);
            return new JsonResult(item, this);
        }
        /// <summary>
        /// Добавит сервер
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Post(ModifyAdminServer model)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var itemId = dbProc.AddServer(model);
            return Get(itemId);
        }
        /// <summary>
        /// Изменить сервер
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Put(int id, ModifyAdminServer model)
        {
            model.Id = id;
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            dbProc.UpdateServer(model);
            return Get(model.Id);
        }
        /// <summary>
        /// Удаление сервера
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Delete(int id)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            dbProc.DeleteServer(id);
            var item = new FrontAdminServer { Id = id };
            return new JsonResult(item, this);
        }
    }
}
