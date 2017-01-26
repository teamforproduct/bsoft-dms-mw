using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.Enums;
using BL.Model.SystemCore.FrontModel;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.WebAPI
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "Licences")]
    public class LicencesController : ApiController
    {
        public IHttpActionResult Get(FilterAspNetLicences filter)
        {
            var dbProc = new WebAPIDbProcess();
            var items = dbProc.GetLicences(filter);
            return new JsonResult(items, this);
        }
        public IHttpActionResult Get(int id)
        {
            var dbProc = new WebAPIDbProcess();
            var item = dbProc.GetLicence(id);
            return new JsonResult(item, this);
        }
        public IHttpActionResult Post(ModifyAspNetLicence model)
        {
            var dbProc = new WebAPIDbProcess();
            var itemId = dbProc.AddLicence(model);
            return Get(itemId);
        }
        public IHttpActionResult Put(int id, ModifyAspNetLicence model)
        {
            model.Id = id;
            var dbProc = new WebAPIDbProcess();
            dbProc.UpdateLicence(model);
            return Get(model.Id);
        }
        public IHttpActionResult Delete(int id)
        {
            var dbProc = new WebAPIDbProcess();
            dbProc.DeleteLicence(id);
            var item = new FrontAspNetLicence { Id = id };
            return new JsonResult(item, this);
        }
        /// <summary>
        /// Проверка лицензии
        /// </summary>
        /// <returns>список должностей</returns>
        [Route("VerifyLicences")]
        [HttpGet]
        [ResponseType(typeof(FrontSystemLicencesInfo))]
        public IHttpActionResult VerifyLicences()
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var res = new FrontSystemLicencesInfo
            {
                MessageLevelTypes = EnumMessageLevelTypes.Red,
                MessageLevelTypesName = EnumMessageLevelTypes.Red.ToString(),
                Message = "Ваша лицензия на V2 заканчивается. Пожалуйста, перейдите на V3", //TODO 
            };
            return new JsonResult(res, this);
        }
    }
}
