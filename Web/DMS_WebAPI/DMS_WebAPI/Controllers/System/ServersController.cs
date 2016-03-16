using BL.Logic.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.FrontModel;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    public class ServersController : ApiController
    {
        /// <summary>
        /// Получение списка доступных серверов
        /// </summary>
        /// <returns>список серверов</returns>
        public IHttpActionResult Get()
        {
            return new JsonResult(new Servers().GetServers(), this);
        }
        /// <summary>
        /// Получение сервера
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Get(int id)
        {
            return new JsonResult(new Servers().GetServer(id), this);
        }
        /// <summary>
        /// Добавит сервер
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Post(ModifyServer model)
        {
            return Get(new Servers().AddServer(model));
        }
        /// <summary>
        /// Изменить сервер
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Put(int id,ModifyServer model)
        {
            model.Id = id;
            new Servers().UpdateServer(model);
            return Get(model.Id);
        }
        /// <summary>
        /// Удаление сервера
        /// </summary>
        /// <returns>сервер</returns>
        public IHttpActionResult Delete(int id)
        {
            new Servers().DeleteServer(id);
            var item = new FrontServer { Id = id };
            return new JsonResult(item, this);
        }
    }
}
