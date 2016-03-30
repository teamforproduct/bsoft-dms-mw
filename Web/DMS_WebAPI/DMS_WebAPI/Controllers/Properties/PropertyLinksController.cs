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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetPropertyLinks(cxt, filter);
            return new JsonResult(tmpItems, this);
        }

        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetPropertyLink(cxt, id);
            return new JsonResult(tmpItems, this);
        }

        public IHttpActionResult Post([FromBody]ModifyPropertyLink model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            return Get((int)tmpServ.ExecuteAction(EnumPropertyAction.AddPropertyLink,  cxt, model));
        }

        public IHttpActionResult Put(int id, [FromBody]ModifyPropertyLink model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            tmpServ.ExecuteAction(EnumPropertyAction.ModifyPropertyLink, cxt, model);
            return Get(model.Id);
        }

        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();

            tmpServ.ExecuteAction(EnumPropertyAction.DeletePropertyLink, cxt, id);
            FrontPropertyLink tmp = new FrontPropertyLink();
            tmp.Id = id;

            return new JsonResult(tmp, this);
            
        }

    }
}