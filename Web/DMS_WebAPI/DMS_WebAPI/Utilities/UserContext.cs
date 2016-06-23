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
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var contextValue = _casheContexts[token];

            try
            {
                var ctx = (IContext)contextValue.StoreObject;

                //TODO Licence
                //if (ctx.ClientLicence?.LicenceError != null)
                //{
                //    throw ctx.ClientLicence.LicenceError as DmsExceptions;
                //}

                //VerifyNumberOfConnections(ctx, ctx.CurrentClientId);

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

            db.ClientId = clientId;

            VerifyNumberOfConnectionsByNew(context, clientId, new List<DatabaseModel> { db });

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

            var dbs = dbProc.GetServersByAdmin(new BL.Model.WebAPI.Filters.FilterAdminServers { ClientIds = new List<int> { client.Id } });

            VerifyNumberOfConnectionsByNew(context, client.Id, dbs);

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

        public void VerifyLicence(int clientId, IEnumerable<DatabaseModel> dbs)
        {
            var clientUsers = _casheContexts
                    .Select(x => (IContext)x.Value.StoreObject)
                    .Where(x => x.CurrentClientId == clientId);

            if (clientUsers.Count() <= 0)
                return;

            var dbProc = new WebAPIDbProcess();

            var lic = dbProc.GetClientLicenceActive(clientId);

            var si = new SystemInfo();
            var regCode = si.GetRegCode(lic);

            if (lic.IsConcurenteLicence)
            {
                var qry = _casheContexts
                   .Select(x => (IContext)x.Value.StoreObject)
                   .Where(x => x.CurrentClientId == clientId)
                   .Select(x => x.CurrentEmployee.UserId)
                   .Distinct();

                lic.ConcurenteNumberOfConnectionsNow = qry.Count();
            }

            object licenceError = null;

            try
            {
                licenceError = new Licences().Verify(regCode, lic, dbs, false);
            }
            catch (Exception ex)
            {
                licenceError = ex;
            }

            foreach (var user in clientUsers)
            {
                user.ClientLicence.LicenceError = licenceError;
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

        public void VerifyNumberOfConnectionsByNew(IContext context, int clientId, IEnumerable<DatabaseModel> dbs)
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
                var qry = _casheContexts
                    .Select(x => (IContext)x.Value.StoreObject)
                    .Where(x => x.CurrentClientId == clientId)
                    .Select(x => x.CurrentEmployee.UserId)
                    .Distinct();

                var count = qry.Count();

                if (!qry.Any(x => x == context.CurrentEmployee.UserId))
                {
                    count++;
                }

                lic.ConcurenteNumberOfConnectionsNow = count;
            }

            var licenceError = new Licences().Verify(regCode, lic, dbs, false);

            if (licenceError!=null)
            {
                if (licenceError is LicenceExceededNumberOfConnectedUsers)
                {
                    throw licenceError as LicenceExceededNumberOfConnectedUsers;
                }
                else
                {
                    context.ClientLicence.LicenceError = licenceError;
                }
            }

        }

        public void ClearCache()
        {
            _casheContexts.Clear();
        }
    }
}
