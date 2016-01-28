using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace DMS_WebAPI.Utilities
{
    public class UserContext
    {
        private readonly Dictionary<string, StoreInfo> _casheContexts = new Dictionary<string, StoreInfo>();
        private const string _TOKEN_KEY = "Authorization";
        private const int _TIME_OUT = 14;
        private string _Token { get { return HttpContext.Current.Request.Headers[_TOKEN_KEY]; } }

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param key="settingName">Setting key.</param>
        /// <returns>Typed setting value.</returns>
        public IContext Get()
        {
            string token = _Token;
            if (!_casheContexts.ContainsKey(token))
            {
                var userManager = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());


                Save(new DefaultContext
                {
                    CurrentEmployee = new BL.Model.Users.Employee
                    {
                        Token = token,
                        AgentId = user.AgentId
                    }
                });
                //throw new Exception();
            }

            var contextValue = _casheContexts[token];
            try
            {
                contextValue.LastUsage = DateTime.Now;
                return (IContext)(contextValue.StoreObject);
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new Exception();
            }
        }

        public void Save(IContext val)
        {
            _casheContexts.Add(_Token, new StoreInfo() { StoreObject = val, LastUsage = DateTime.Now });
        }

        public void ClearCache()
        {
            _casheContexts.Clear();
        }
    }
}
