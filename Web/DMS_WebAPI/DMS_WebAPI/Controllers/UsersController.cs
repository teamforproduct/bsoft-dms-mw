using BL.CrossCutting.DependencyInjection;
using BL.Model.Users;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/v2/Users")]
    public class UsersController : ApiController
    {
        // POST: api/Users/Position
        [Route("Position")]
        public IHttpActionResult Post([FromBody]int positionId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            cxt.CurrentPosition = new List<Position>() { new Position { PositionId = positionId } };
            return new JsonResult(null, this);
        }

        // GET: api/Users/Servers
        [Route("Servers")]
        [AllowAnonymous]
        public IHttpActionResult GetServers()
        {
            var readXml = new ReadXml("/servers.xml");
            return new JsonResult(readXml.ReadDBsByUI(), this);
        }
    }
}
