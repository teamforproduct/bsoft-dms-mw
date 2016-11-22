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
using BL.Model.SystemCore;
using BL.Model.WebAPI.FrontModel;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Коллекция пользовательских контекстов
    /// </summary>
    public class UserContexts
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

                var request_ctx = new UserContext(ctx);
                request_ctx.SetCurrentPosition(null);
                return request_ctx;
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new Exception();
            }
        }

        public IQueryable<FrontSystemSession> GetContextListQuery()
        {
            var res = _casheContexts.AsQueryable()
                .Where(x => x.Value.StoreObject is IContext)
                .Select(x => new FrontSystemSession
                {
                    Token = x.Key,
                    LastUsage  = x.Value.LastUsage,
                    CreateDate = (x.Value.StoreObject as IContext).CreateDate,
                    LoginLogInfo = (x.Value.StoreObject as IContext).LoginLogInfo,
                    LoginLogId = (x.Value.StoreObject as IContext).LoginLogId,
                    UserId = (x.Value.StoreObject as IContext).CurrentEmployee.UserId,
                    AgentId = (x.Value.StoreObject as IContext).CurrentEmployee.AgentId,
                    Name = (x.Value.StoreObject as IContext).CurrentEmployee.Name,
                    ClientId = (x.Value.StoreObject as IContext).CurrentEmployee.ClientId,
                    IsActive = true,
                    
                });
            return res;
        }

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <param name="currentPositionId"></param>
        /// <param name="isThrowExeception"></param>
        /// <returns>Typed setting value.</returns>
        public IContext Get(int? currentPositionId = null, bool isThrowExeception = true)
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var storeInfo = _casheContexts[token];

            try
            {
                var ctx = (IContext)storeInfo.StoreObject;

                //TODO Licence
                //if (ctx.ClientLicence?.LicenceError != null)
                //{
                //    throw ctx.ClientLicence.LicenceError as DmsExceptions;
                //}

                //VerifyNumberOfConnections(ctx, ctx.CurrentClientId);

                // KeepAlive: Продление жизни пользовательского контекста
                storeInfo.LastUsage = DateTime.UtcNow;

                var request_ctx = new UserContext(ctx);
                request_ctx.SetCurrentPosition(currentPositionId);

                if (isThrowExeception && request_ctx.IsChangePasswordRequired)
                    throw new ChangePasswordRequiredAgentUser();

                return request_ctx;
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Удаляет пользовательский контекст из коллекции
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
        
        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №1
        /// Добавляет пользовательский контекст с базовыми параметрами (token, userId, clientCode)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId">Id Web-пользователя</param>
        /// <param name="clientCode">доменное имя клиента</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IContext Set(string token, string userId, string clientCode, bool IsChangePasswordRequired)
        {
            token = token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                var context =
                new UserContext
                {
                    CurrentEmployee = new BL.Model.Users.Employee
                    {
                        Token = token,
                        UserId = userId,
                        ClientCode = clientCode
                    }
                };

                context.IsChangePasswordRequired = IsChangePasswordRequired;

                Save(token, context);
                return context;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №2
        /// Добавляет к существующему пользовательскому контексту доступные лицензии, указанную базу, профиль пользователя
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

            var storeInfo = _casheContexts[token];

            var context = (IContext)storeInfo.StoreObject;

            var dbProc = new WebAPIDbProcess();
            context.ClientLicence = dbProc.GetClientLicenceActive(clientId);

            db.ClientId = clientId;

            VerifyNumberOfConnectionsByNew(context, clientId, new List<DatabaseModel> { db });

            // KeepAlive: Продление жизни пользовательского контекста
            storeInfo.LastUsage = DateTime.UtcNow;

            context.CurrentClientId = clientId;

            context.CurrentDB = db;

            var agentUser = DmsResolver.Current.Get<IAdminService>().GetUserForContext(context, context.CurrentEmployee.UserId);

            if (agentUser != null)
            {
                // решили не использовать флаг IsActive
                //if (!agentUser.IsActive) throw new UserIsDeactivated(agentUser.Name);

                if (agentUser.PositionExecutorsCount == 0) throw new UserNotExecuteAnyPosition(agentUser.Name);

                context.CurrentEmployee.AgentId = agentUser.AgentId;
                context.CurrentEmployee.Name = agentUser.Name;
                context.CurrentEmployee.LanguageId = agentUser.LanguageId;
            }
            else
            {
                throw new UserAccessIsDenied();
            }

        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Добавляет к существующему пользовательскому контексту информации по логу
        /// </summary>
        /// <param name="db">new server parameters</param>
        /// <param name="clientId">clientId</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void Set(int? loginLogId, string loginLogInfo)
        {
            string token = Token.ToLower();
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var storeInfo = _casheContexts[token];

            var context = (IContext)storeInfo.StoreObject;
            context.LoginLogId = loginLogId;
            context.LoginLogInfo = loginLogInfo;
        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №3
        /// Добавляет к существующему пользовательскому контексту список занимаемых должностей и AccessLevel
        /// </summary>
        /// <param name="token"></param>
        /// <param name="positionsIdList"></param>
        public void SetUserPositions(string token, List<int> positionsIdList)
        {
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var storeInfo = _casheContexts[token];

            // KeepAlive: Продление жизни пользовательского контекста
            storeInfo.LastUsage = DateTime.UtcNow;
            var context = (IContext)storeInfo.StoreObject;
            context.CurrentPositionsIdList = positionsIdList;
            context.CurrentPositionsAccessLevel = DmsResolver.Current.Get<IAdminService>().GetCurrentPositionsAccessLevel(context);
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

            var storeInfo = _casheContexts[token];

            var context = (IContext)storeInfo.StoreObject;

            var dbProc = new WebAPIDbProcess();
            context.ClientLicence = dbProc.GetClientLicenceActive(client.Id);

            var dbs = dbProc.GetServersByAdmin(new BL.Model.WebAPI.Filters.FilterAdminServers { ClientIds = new List<int> { client.Id } });

            VerifyNumberOfConnectionsByNew(context, client.Id, dbs);

            // KeepAlive: Продление жизни пользовательского контекста
            storeInfo.LastUsage = DateTime.UtcNow;

            context.CurrentClientId = client.Id;
        }

        /// <summary>
        /// UpdateChangePasswordRequired
        /// </summary>
        /// <param name="IsChangePasswordRequired"></param>
        /// <param name="userId">Id Web-пользователя</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateChangePasswordRequired(string userId, bool IsChangePasswordRequired)
        {
            var keys = _casheContexts.Where(x => { try { return ((IContext)x.Value.StoreObject).CurrentEmployee.UserId == userId; } catch { } return false; }).Select(x => x.Key).ToArray();
            foreach (var key in keys)
            {
                try
                {
                    ((IContext)(_casheContexts[key].StoreObject)).IsChangePasswordRequired = IsChangePasswordRequired;
                }
                catch { }
            }
        }

        private void Save(IContext val)
        {
            _casheContexts.Add(Token.ToLower(), new StoreInfo() { StoreObject = val, LastUsage = DateTime.UtcNow });
        }
        private void Save(string token, IContext val)
        {
            _casheContexts.Add(token.ToLower(), new StoreInfo() { StoreObject = val, LastUsage = DateTime.UtcNow });
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
            var now = DateTime.UtcNow;
            var keys = _casheContexts.Where(x => x.Value.LastUsage.AddDays(_TIME_OUT) <= now).Select(x => x.Key).ToArray();
            foreach (var key in keys)
            {
                _casheContexts.Remove(key);
            }
        }

        public void KillSessions(int agentId)
        {
            var now = DateTime.UtcNow;
            var keys = _casheContexts.Where(x => { try { return ((IContext)x.Value.StoreObject).CurrentAgentId == agentId; } catch { } return false; }).Select(x => x.Key).ToArray();
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

            if (licenceError != null)
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

        public void UpdateLanguageId(int agentId, int languageId)
        {
            var contexts = _casheContexts
                        .Select(x => (IContext)x.Value.StoreObject)
                        .Where(x => x.CurrentEmployee.AgentId == agentId).ToList();

            foreach (var context in contexts)
            {
                context.CurrentEmployee.LanguageId = languageId;
            }
        }

        /// <summary>
        /// Очистка всех пользовательских контекстов
        /// </summary>
        public void ClearCache()
        {
            _casheContexts.Clear();
        }

        /// <summary>
        /// Количество активных пользователей
        /// </summary>
        public int Count
        {
            get { return _casheContexts.Count; }
        }

    }
}
