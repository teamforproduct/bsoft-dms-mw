using BL.Model.Database;
using System;
using System.Collections.Generic;
using System.Web;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Exception;
using System.Linq;
using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;

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
        /// Remove setting value by its name.
        /// </summary>
        /// <returns>Typed setting value.</returns>
        public IContext Remove(string token = null)
        { 
            if (string.IsNullOrEmpty(token)) token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                return null;
            }

            var contextValue = _casheContexts[token];
            try
            {
                var cxt = (IContext)contextValue.StoreObject;
                _casheContexts.Remove(token);
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
        public IContext Set(string token, DatabaseModel db, string userId, bool isSuperAdmin)
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
                    CurrentDB = db
                };

                if (!(db==null && isSuperAdmin))
                {
                    var agent = DmsResolver.Current.Get<IAdminService>().GetEmployee(context, userId);

                    if (agent != null)
                    {
                        context.CurrentEmployee.AgentId = agent.AgentId;
                        context.CurrentEmployee.Name = agent.Name;
                        context.CurrentEmployee.LanguageId = agent.LanguageId;
                    }
                    else if (!isSuperAdmin)
                    {
                        throw new AccessIsDenied();
                    }
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

        public void RemoveByTimeout()
        {
            var now = DateTime.Now;
            var keys = _casheContexts.Where(x => x.Value.LastUsage.AddDays(_TIME_OUT) <= now).Select(x => x.Key).ToArray();
            foreach (var key in keys)
            {
                _casheContexts.Remove(key);
            }
        }

        public void ClearCache()
        {
            _casheContexts.Clear();
        }
    }
}
