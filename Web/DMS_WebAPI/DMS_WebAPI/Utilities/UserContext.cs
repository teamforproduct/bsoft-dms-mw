using BL.Model.Database;
using System;
using System.Collections.Generic;
using System.Web;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Exception;
using System.Linq;
using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;

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
                var cxt = (IContext)contextValue.StoreObject;

                VerifyNumberOfConnections(cxt);

                contextValue.LastUsage = DateTime.Now;

                var request_ctx = new DefaultContext(cxt);
                request_ctx.SetCurrentPosition(currentPositionId);
                return request_ctx;
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

        public void SetUserPositions(string token, List<int> positionsIdList)
        {
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];

            contextValue.LastUsage = DateTime.Now;
            var context = (IContext)contextValue.StoreObject;
            context.CurrentPositionsIdList = positionsIdList;
        }


        /// <summary>
        /// Add new server to the list of available servers
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IContext Set(string token, string userId)
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
                    }
                };
                //TODO insert here licence INFO 
                Save(token, context);
                return context;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Add new server to the list of available servers
        /// </summary>
        /// <param name="db">new server parameters</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void Set(DatabaseModel db)
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];

            var context = (IContext)contextValue.StoreObject;

            VerifyNumberOfConnections(context, true);

            contextValue.LastUsage = DateTime.Now;

            context.CurrentDB = db;

            var agent = DmsResolver.Current.Get<IAdminService>().GetEmployee(context, context.CurrentEmployee.UserId);

            if (agent != null)
            {
                context.CurrentEmployee.AgentId = agent.AgentId;
                context.CurrentEmployee.Name = agent.Name;
                context.CurrentEmployee.LanguageId = agent.LanguageId;
            }
            else
            {
                throw new AccessIsDenied();
            }

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

        public void VerifyNumberOfConnections(IContext context, bool isAddNew = false)
        {
            if (context == null) return;
            var clientId = context.CurrentClientId;
            if (!clientId.HasValue) return;

            var si = new SystemInfo();
            var dbw = new SystemDbWorker();

            var lic = context.ClientLicence;

            if (lic == null)
                lic = dbw.GetLicenceInfo(clientId.GetValueOrDefault());

            var regCode = si.GetRegCode(lic);

            if (lic.IsConcurenteLicence)
            {
                var now = DateTime.Now.AddMinutes(-5);
                var count = _casheContexts
                    .Where(x => x.Value.LastUsage > now)
                    .Select(x => (IContext)x.Value.StoreObject)
                    .Count(x => x.CurrentClientId == clientId);

                if (isAddNew) count++;

                lic.ConcurenteNumberOfConnectionsNow = count;
            }

            VerifyLicence.Verify(regCode, lic);

        }

        public void ClearCache()
        {
            _casheContexts.Clear();
        }
    }
}
