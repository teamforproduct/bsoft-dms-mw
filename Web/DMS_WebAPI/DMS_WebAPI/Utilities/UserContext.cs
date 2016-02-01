using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using System;
using System.Collections.Generic;
using System.Web;
using BL.CrossCutting.DependencyInjection;
using BL.Database.DatabaseContext;
using BL.Logic.Secure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DMS_WebAPI.Utilities
{
    public class UserContext
    {
        private readonly Dictionary<string, StoreInfo> _casheContexts = new Dictionary<string, StoreInfo>();
        private const string _TOKEN_KEY = "Authorization";
        private const int _TIME_OUT = 14;
        private string Token { get { return HttpContext.Current.Request.Headers[_TOKEN_KEY]; } }

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <typeparam name="T">Expected setting value type.</typeparam>
        /// <param key="settingName">Setting key.</param>
        /// <returns>Typed setting value.</returns>
        public IContext Get()
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new HttpException(401, "Unauthorized access");
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

        public IContext Set(string token, DatabaseModel db)
        {
            token = token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                var userManager = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());
                var context = 
                new DefaultContext
                {
                    CurrentEmployee = new BL.Model.Users.Employee
                    {
                        Token = token,
                        AgentId = user.AgentId
                    },
                    CurrentDB = db
                };

                if (user.AgentId.HasValue)
                {
                    var agent = DmsResolver.Current.Get<ISecureService>().GetEmployee(context, user.AgentId.Value);
                    context.CurrentEmployee.Name = agent.Name;
                }

                Save(token, context);
                return context;
            }

            throw new ArgumentException();
        }

        private void Save(IContext val)
        {
            _casheContexts.Add(Token.ToLower(), new StoreInfo() { StoreObject = val, LastUsage = DateTime.Now });
        }
        private void Save(string token, IContext val)
        {
            _casheContexts.Add(token.ToLower(), new StoreInfo() { StoreObject = val, LastUsage = DateTime.Now });
        }

        public void ClearCache()
        {
            _casheContexts.Clear();
        }
    }
}
