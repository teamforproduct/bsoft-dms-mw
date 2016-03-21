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
using BL.Model.Database.InternalModel;

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

        public IContext Set(string token, InternalServer db, string userId)
        {
            token = token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                var context =
                new DefaultContext
                {
                    CurrentEmployee = new BL.Model.Users.Employee
                    {
                        Token = token,
                        UserId = userId
                    },
                    CurrentDB = new DatabaseModel(db)
                };

                var agent = DmsResolver.Current.Get<IAdminService>().GetEmployee(context, userId);

                if (agent == null)
                {
                    throw new AccessIsDenied();
                }
                context.CurrentEmployee.AgentId = agent.AgentId;
                context.CurrentEmployee.Name = agent.Name;
                context.CurrentEmployee.LanguageId = agent.LanguageId;

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
