using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using JsonResult = DMS_WebAPI.Results.JsonResult;
using ModelStateDictionary = System.Web.Http.ModelBinding.ModelStateDictionary;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class WebApiController:ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <param name="addittionalParameters"></param>
        /// <returns></returns>
        public Task<IHttpActionResult> SafeExecuteAsync(ModelStateDictionary state, Func<IContext, object, IHttpActionResult> action, object addittionalParameters = null)
        {
//            try
            {
                if (state !=null && !state.IsValid)
                {
                    return Task.Factory.StartNew(() => PrepareResponse("Incoming model not valid. ModelState: " + state, (int)HttpStatusCode.BadRequest));
                }

                var context = DmsResolver.Current.Get<UserContexts>().Get();
                return Task.Factory.StartNew(() => ExecuteAction(context, action, addittionalParameters));
            }
            //catch (Exception ex)
            //{
            //    FileLogger.AppendTextToFile($"SafeExecuteAsync: Message={ex.Message}  STACK={ex.StackTrace}", "C:\\err.txt" );
            //    throw ex;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        /// <param name="addittionalParameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IHttpActionResult ExecuteAction(IContext context, Func<IContext, object, IHttpActionResult> action, object addittionalParameters)
        {
            //try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var ret = action(context, addittionalParameters);
                stopWatch.Stop();
                var res = ret as JsonResult;
                if (res != null)
                {
                    res.SpentTime = stopWatch;
                }
                return ret;
            }
            //catch (Exception ex)
            //{
            //    FileLogger.AppendTextToFile($"ExecuteAction: Message={ex.Message}  STACK={ex.StackTrace}", "C:\\err.txt");
            //    throw ex;
            //    //  return PrepareResponse(ex.Message, 422);
            //}


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

        private IHttpActionResult PrepareResponse(string responseBody, int rc)
        {
            var response = new HttpResponseMessage((HttpStatusCode)rc)
            {
                Content = new StringContent(responseBody)
            };
            return new ResponseMessageResult(response);
        }

    }
}