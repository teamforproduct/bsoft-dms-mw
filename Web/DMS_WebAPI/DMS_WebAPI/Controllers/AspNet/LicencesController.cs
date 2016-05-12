using BL.Model.AspNet.FrontModel;
using BL.Model.AspNet.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities.AspNet;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.AspNet
{
    public class LicencesController : ApiController
    {
        /// <summary>
        /// Получение списка лицензий
        /// </summary>
        /// <returns>список клиентов</returns>
        public IHttpActionResult Get()
        {
            return new JsonResult(new AspNetLicences().GetLicences(), this);
        }
        /// <summary>
        /// Получение клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Get(int id)
        {
            return new JsonResult(new AspNetLicences().GetLicence(id), this);
        }
        /// <summary>
        /// Добавит клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Post(ModifyAspNetLicence model)
        {
            return Get(new AspNetLicences().AddLicence(model));
        }
        /// <summary>
        /// Изменить клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Put(int id, ModifyAspNetLicence model)
        {
            model.Id = id;
            new AspNetLicences().UpdateLicence(model);
            return Get(model.Id);
        }
        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <returns>клиент</returns>
        public IHttpActionResult Delete(int id)
        {
            new AspNetLicences().DeleteLicence(id);
            var item = new FrontAspNetLicence { Id = id };
            return new JsonResult(item, this);
        }
    }
}
