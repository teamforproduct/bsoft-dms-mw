using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.Database.IncomingModel;
using BL.Model.Database.FrontModel;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/v2/Servers")]
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

        /// <summary>
        /// Реиндексация полнотекстового поиска для сервера
        /// </summary>
        /// <returns>сервер</returns>
        [Route("FullTextReindex")]
        [HttpPost]
        public IHttpActionResult FullTextReindex(int id)
        {
            var srv = new Servers().GetServer(id);
            var ctx = new AdminContext(srv);
            var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
            ftService.ReindexDatabase(ctx);
            return new JsonResult(new FrontServer { Id = id }, this);
        }
    }
}
