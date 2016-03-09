using BL.Logic.DependencyInjection;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Logic.PropertyCore.Interfaces;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.FrontModel;

namespace DMS_WebAPI.Controllers.Properties
{
    [Authorize]
    public class PropertyValuesController : ApiController
    {
        public IHttpActionResult Get([FromUri] FilterPropertyValue filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetPropertyValues(cxt, filter);
            return new JsonResult(tmpItems, this);
        }

        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            var tmpItems = tmpServ.GetPropertyValue(cxt, id);
            return new JsonResult(tmpItems, this);
        }

        public IHttpActionResult Post([FromBody]ModifyPropertyValue model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            return Get((int)tmpServ.ExecuteAction(EnumPropertyAction.AddPropertyValue,  cxt, model));
        }

        public IHttpActionResult Put(int id, [FromBody]ModifyPropertyValue model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();
            tmpServ.ExecuteAction(EnumPropertyAction.ModifyPropertyValues, cxt, model);
            return Get(model.Id);
        }

        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpServ = DmsResolver.Current.Get<IPropertyService>();

            tmpServ.ExecuteAction(EnumPropertyAction.DeletePropertyValue, cxt, id);
            FrontPropertyValue tmp = new FrontPropertyValue();
            tmp.Id = id;

            return new JsonResult(tmp, this);
            
        }

    }
}