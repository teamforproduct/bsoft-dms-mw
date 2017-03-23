using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using BL.CrossCutting.Interfaces;
using JsonResult = DMS_WebAPI.Results.JsonResult;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiHelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <param name="addittionalParameters"></param>
        /// <returns></returns>
        public static Task<IHttpActionResult> SafeExecuteAsync(this ApiController ctrl, ModelStateDictionary state, IContext context, Func<IContext, IHttpActionResult> action, object addittionalParameters = null)
        {
            try
            {
                return Task.Factory.StartNew(() => ExecuteAction(context, action, addittionalParameters));
            }
            catch (Exception ex)
            {
                throw ex;
                //var logger = DmsResolver.Current.Get<ILogger>();
                //logger.Fatal(ex, );
                //return ctrl.PrepareResponse(ex.Message, 422);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="addittionalParameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHttpActionResult ExecuteAction(IContext context, Func<IContext, IHttpActionResult> action, object addittionalParameters)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var ret = action(context);
                stopWatch.Stop();
                var res = ret as JsonResult;
                if (res != null)
                {
                    res.SpentTime = stopWatch;
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
                //  return PrepareResponse(ex.Message, 422);
            }


            //var name = string.Empty;
            //try
            //{
            //    name = action.Method.Name + (action.Method.ReflectedType != null ? action.Method.ReflectedType.FullName : "");
            //}
            //catch (Exception)
            //{
            //    name = "Exception by get action.Method.Name + (action.Method.ReflectedType != null ? action.Method.ReflectedType.FullName : \"\");";
            //}
        }

        private static IHttpActionResult PrepareResponse(string responseBody, int rc)
        {
            var response = new HttpResponseMessage((HttpStatusCode)rc)
            {
                Content = new StringContent(responseBody)
            };
            return new ResponseMessageResult(response);
        }

        public static IHttpActionResult PrepareResponse(this ApiController ctrl, string responseBody, int rc)
        {
           
            return PrepareResponse(responseBody, rc);
        }
    }
}