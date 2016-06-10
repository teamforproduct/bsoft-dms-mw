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
using BL.Model.WebAPI.FrontModel;

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
        /// <returns>Typed setting value.</returns>
        public IContext GetByLanguage()
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];
            try
            {
                var ctx = (IContext)contextValue.StoreObject;

                var request_ctx = new DefaultContext(ctx);
                request_ctx.SetCurrentPosition(null);
                return request_ctx;
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <param name="currentPositionId"></param>
        /// <returns>Typed setting value.</returns>
        public IContext Get(int? currentPositionId = null)
        {
            var time = new System.Diagnostics.Stopwatch();

            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];

            try
            {
                var ctx = (IContext)contextValue.StoreObject;

                if (!(ctx.ClientLicence?.IsActive??true))
                {
                    throw new LicenceError();
                }

                //time.Start();
                //VerifyNumberOfConnections(ctx, ctx.CurrentClientId);
                //time.Stop();
                //BL.CrossCutting.Helpers.Logger.SaveToFile("UC:UserContext", time.Elapsed);
                //time.Reset();

                contextValue.LastUsage = DateTime.Now;

                var request_ctx = new DefaultContext(ctx);
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
                var ctx = (IContext)contextValue.StoreObject;
                _casheContexts.Remove(token);
                return ctx;
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
        public IContext Set(string token, string userId, string clientCode)
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
                        UserId = userId,
                        ClientCode = clientCode
                    }
                };
                Save(token, context);
                return context;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Add new server to the list of available servers
        /// </summary>
        /// <param name="db">new server parameters</param>
        /// <param name="clientId">clientId</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void Set(DatabaseModel db, int clientId)
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];

            var context = (IContext)contextValue.StoreObject;

            var dbProc = new WebAPIDbProcess();
            context.ClientLicence = dbProc.GetClientLicenceActive(clientId);

            VerifyNumberOfConnections(context, clientId, true);

            contextValue.LastUsage = DateTime.Now;

            context.CurrentClientId = clientId;

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

        /// <summary>
        /// Set client
        /// </summary>
        /// <param name="client">new client parameters</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void Set(FrontAspNetClient client)
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];

            var context = (IContext)contextValue.StoreObject;

            var dbProc = new WebAPIDbProcess();
            context.ClientLicence = dbProc.GetClientLicenceActive(client.Id);

            VerifyNumberOfConnections(context, client.Id, true);

            contextValue.LastUsage = DateTime.Now;

            context.CurrentClientId = client.Id;
        }

        private void Save(IContext val)
        {
            _casheContexts.Add(Token.ToLower(), new StoreInfo() { StoreObject = val, LastUsage = DateTime.Now });
        }
        private void Save(string token, IContext val)
        {
            _casheContexts.Add(token.ToLower(), new StoreInfo() { StoreObject = val, LastUsage = DateTime.Now });
        }

        public void VerifyLicence(int clientId)
        {
            var clientUsers = _casheContexts
                    .Select(x => (IContext)x.Value.StoreObject)
                    .Where(x => x.CurrentClientId == clientId);

            if (clientUsers.Count() <= 0)
                return;

            var context = clientUsers.FirstOrDefault();

            var dbProc = new WebAPIDbProcess();

            var lic = dbProc.GetClientLicenceActive(clientId);

            var si = new SystemInfo();
            var regCode = si.GetRegCode(lic);

            if (lic.IsConcurenteLicence)
            {
                var now = DateTime.Now.AddMinutes(-5);
                var qry = _casheContexts
                    .Where(x => x.Value.LastUsage > now)
                    .Select(x => (IContext)x.Value.StoreObject)
                    .Where(x => x.CurrentClientId == clientId);

                lic.ConcurenteNumberOfConnectionsNow = qry.Count();
            }

            var isVerifyLicence = true;

            try
            {
                new Licences().Verify(regCode, lic, context);
            }
            catch (LicenceError)
            {
                isVerifyLicence = false;
            }
            catch (LicenceExpired)
            {
                isVerifyLicence = false;
            }
            catch (LicenceExceededNumberOfRegisteredUsers)
            {
                isVerifyLicence = false;
            }
            catch (LicenceExceededNumberOfConnectedUsers)
            {
                isVerifyLicence = false;
            }
            catch (Exception ex)
            {
                isVerifyLicence = false;
            }

            if (!isVerifyLicence)
            {
                foreach (var user in clientUsers)
                {
                    user.ClientLicence.IsActive = false;
                }
            }
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

        public void VerifyNumberOfConnections(IContext context, int clientId, bool isAddNew = false)
        {
            if (clientId <= 0) return;

            var si = new SystemInfo();

            var lic = context.ClientLicence;

            if (lic == null)
            {
                var dbProc = new WebAPIDbProcess();
                context.ClientLicence = lic = dbProc.GetClientLicenceActive(clientId);
            }

            var regCode = si.GetRegCode(lic);

            if (lic.IsConcurenteLicence)
            {
                var now = DateTime.Now.AddMinutes(-5);
                var qry = _casheContexts
                    .Where(x => x.Value.LastUsage > now)
                    .Select(x => (IContext)x.Value.StoreObject)
                    .Where(x => x.CurrentClientId == clientId);

                if (isAddNew)
                {
                    qry = qry.Where(x => x.CurrentEmployee.Token != context.CurrentEmployee.Token);
                }

                var count = qry.Count();

                if (isAddNew) count++;

                lic.ConcurenteNumberOfConnectionsNow = count;
            }

            new Licences().Verify(regCode, lic, context);

        }

        public void ClearCache()
        {
            _casheContexts.Clear();
        }
    }
}
