using BL.Logic.DependencyInjection;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Logic.PropertyCore.Interfaces;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.FrontModel;

namespace DMS_WebAPI.Controllers.Properties
{
    [Authorize]
    public class PropertyLinksController : ApiController
    {
        public IHttpActionResult Get([FromUri] FilterPropertyLink filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetPropertyLinks(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetPropertyLink(ctx, id);
            return new JsonResult(tmpItems, this);
        }

        public IHttpActionResult Post([FromBody]ModifyPropertyLink model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            return Get((int)tmpServ.ExecuteAction(EnumPropertyAction.AddPropertyLink,  ctx, model));
        }

        public IHttpActionResult Put(int id, [FromBody]ModifyPropertyLink model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            tmpServ.ExecuteAction(EnumPropertyAction.ModifyPropertyLink, ctx, model);
            return Get(model.Id);
        }

        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();

            tmpServ.ExecuteAction(EnumPropertyAction.DeletePropertyLink, ctx, id);
            FrontPropertyLink tmp = new FrontPropertyLink();
            tmp.Id = id;

            return new JsonResult(tmp, this);
            
        }

    }
}