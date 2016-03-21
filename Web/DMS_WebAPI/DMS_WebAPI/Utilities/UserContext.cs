using BL.Logic.Context;
using BL.Model.Database;
using System;
using System.Collections.Generic;
using System.Web;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using BL.Model.Exception;


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
        /// <param name="currentPositionId"></param>
        /// <returns>Typed setting value.</returns>
        public IContext Get(int? currentPositionId = null)
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];
            try
            {
                contextValue.LastUsage = DateTime.Now;
                var cxt = (IContext)contextValue.StoreObject;
                cxt.SetCurrentPosition(currentPositionId);
                return cxt;
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Add new server to the list of available servers
        /// </summary>
        /// <param name="token"></param>
        /// <param name="db">new server parameters</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IContext Set(string token, DatabaseModel db, string userId)
        {
            token = token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                var userManager = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = userManager.FindById(userId);
                var context = 
                new DefaultContext
                {
                    CurrentEmployee = new BL.Model.Users.Employee
                    {
                        Token = token,
                        AgentId = user.AgentId
                    },
                    CurrentDB = new DatabaseModel
                    {
                        Id = db.Id,
                        Address = db.Address,
                        Name = db.Name,
                        ServerType = db.ServerType,
                        DefaultDatabase = db.DefaultDatabase,
                        IntegrateSecurity = db.IntegrateSecurity,
                        UserName = db.UserName,
                        UserPassword = db.UserPassword,
                        ConnectionString = db.ConnectionString
                    }
                };

                if (user.AgentId.HasValue)
                {
                    var agent = DmsResolver.Current.Get<IAdminService>().GetEmployee(context, user.AgentId.Value);
                    context.CurrentEmployee.Name = agent.Name;
                    context.CurrentEmployee.LanguageId = agent.LanguageId;
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
