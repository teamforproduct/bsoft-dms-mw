﻿using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.AspNet.IncomingModel;
using BL.Model.AspNet.FrontModel;

namespace DMS_WebAPI.Controllers.AspNet
{
    [Authorize]
    public class UserServersController : ApiController
    {
        /// <summary>
        /// Получение списка клиентов
        /// </summary>
        /// <returns>список клиентов</returns>
        public IHttpActionResult Get()
        {
            return new JsonResult(new AspNetClients().GetClients(), this);
        }
        /// <summary>
        /// Получение клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Get(int id)
        {
            return new JsonResult(new AspNetClients().GetClient(id), this);
        }
        /// <summary>
        /// Добавит клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Post(ModifyAspNetClient model)
        {
            return Get(new AspNetClients().AddClient(model));
        }
        /// <summary>
        /// Изменить клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Put(int id, ModifyAspNetClient model)
        {
            model.Id = id;
            new AspNetClients().UpdateClient(model);
            return Get(model.Id);
        }
        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Delete(int id)
        {
            new AspNetClients().DeleteClient(id);
            var item = new FrontAspNetClient { Id = id };
            return new JsonResult(item, this);
        }
    }
}