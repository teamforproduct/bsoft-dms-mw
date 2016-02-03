using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Admins
{
    [Authorize]
    [RoutePrefix("api/AdminAccessLevels")]
    public class AdminAccessLevelsController : ApiController
    {
        // GET: api/AdminAccessLevels
        public IHttpActionResult Get([FromUri] FilterAdminAccessLevel filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpAdminProc = DmsResolver.Current.Get<IAdminService>();
            var tmpAdmin = tmpAdminProc.GetAdminAccessLevels(cxt, filter);
            return new JsonResult(tmpAdmin, this);
        }

        // GET: api/AdminAccessLevels/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpAdminProc = DmsResolver.Current.Get<IAdminService>();
            var tmpAdmin = tmpAdminProc.GetAdminAccessLevel(cxt, id);
            return new JsonResult(tmpAdmin, this);
        }
    }
}