using BL.CrossCutting.DependencyInjection;
using BL.Model.Users;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users")]
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

        // GET: api/Users
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Users/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Users
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Users/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }
    }
}
