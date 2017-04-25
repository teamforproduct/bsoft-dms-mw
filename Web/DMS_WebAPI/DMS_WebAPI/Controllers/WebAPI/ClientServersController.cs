using System.Web.Http;

namespace DMS_WebAPI.Controllers.WebAPI
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "ClientServers")]
    public class ClientServersController : ApiController
    {
        //public IHttpActionResult Get(FilterAspNetClientServers filter)
        //{
        //    var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
        //    var items = dbProc.GetClientServers(filter);
        //    return new JsonResult(items, this);
        //}

        //public IHttpActionResult Get(int id)
        //{
        //    var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
        //    var item = dbProc.GetClientServer(id);
        //    return new JsonResult(item, this);
        //}

        //public IHttpActionResult Post(ModifyAspNetClientServer model)
        //{
        //    var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
        //    var itemId = dbProc.AddClientServer(model);
        //    return Get(itemId);
        //}
        //public IHttpActionResult Delete(int id)
        //{
        //    var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
        //    //dbProc.DeleteClientServer(id);
        //    var item = new FrontAspNetClientServer { Id = id };
        //    return new JsonResult(item, this);
        //}

    }
}
