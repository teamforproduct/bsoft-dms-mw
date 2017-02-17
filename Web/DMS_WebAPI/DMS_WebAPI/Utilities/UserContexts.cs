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
using Newtonsoft.Json;
using BL.CrossCutting.Helpers;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.WebAPI.IncomingModel;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Коллекция пользовательских контекстов
    /// </summary>
    public class UserContexts //: IDisposable
    {
        private readonly Dictionary<string, StoreInfo> _casheContexts = new Dictionary<string, StoreInfo>();
        private const string _TOKEN_KEY = "Authorization";
        private const int _TIME_OUT_MIN = 15;
        private string Token { get { return HttpContext.Current.Request.Headers[_TOKEN_KEY]; } }

        private string TokenLower { get { return string.IsNullOrEmpty(Token) ? string.Empty : Token.ToLower(); } }

        ///// <summary>
        ///// Gets setting value by its name.
        ///// </summary>
        ///// <returns>Typed setting value.</returns>
        //public IContext GetByLanguage()
        //{
        //    string token = TokenLower;

        //    if (!Contains(token)) throw new UserUnauthorized();

        //    var contextValue = _casheContexts[token];
        //    try
        //    {
        //        var ctx = (IContext)contextValue.StoreObject;

        //        var request_ctx = new UserContext(ctx);
        //        request_ctx.SetCurrentPosition(null);
        //        return request_ctx;
        //    }
        //    catch (InvalidCastException invalidCastException)
        //    {
        //        throw new Exception();
        //    }
        //}



        public IQueryable<FrontSystemSession> GetContextListQuery()
        {
            var res = _casheContexts.AsQueryable()
                .Where(x => x.Value.StoreObject is IContext)
                .Select(x => new FrontSystemSession
                {
                    //Token = x.Key,
                    LastUsage = x.Value.LastUsage,
                    CreateDate = (x.Value.StoreObject as IContext).CreateDate,
                    LoginLogInfo = (x.Value.StoreObject as IContext).LoginLogInfo,
                    LoginLogId = (x.Value.StoreObject as IContext).LoginLogId,
                    UserId = (x.Value.StoreObject as IContext).CurrentEmployee.UserId,
                    AgentId = (x.Value.StoreObject as IContext).CurrentEmployee.AgentId,
                    Name = (x.Value.StoreObject as IContext).CurrentEmployee.Name,
                    ClientId = (x.Value.StoreObject as IContext).CurrentEmployee.ClientId,
                    IsActive = true,
                    IsSuccess = true,
                });
            return res;
        }



        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <param name="currentPositionId"></param>
        /// <param name="isThrowExeception"></param>
        /// <param name="keepAlive"></param>
        /// <returns>Typed setting value.</returns>
        public IContext Get(int? currentPositionId = null, bool isThrowExeception = true, bool keepAlive = true)
        {
            string token = TokenLower;

            if (!Contains(token)) throw new UserUnauthorized();

            var ctx = GetInternal(token);


            //TODO Licence
            //if (ctx.ClientLicence?.LicenceError != null)
            //{
            //    throw ctx.ClientLicence.LicenceError as DmsExceptions;
            //}

            //VerifyNumberOfConnections(ctx, ctx.CurrentClientId);

            var request_ctx = new UserContext(ctx);
            request_ctx.SetCurrentPosition(currentPositionId);

            if (isThrowExeception && request_ctx.IsChangePasswordRequired)
                throw new UserMustChangePassword();

            // KeepAlive: Продление жизни пользовательского контекста
            if (keepAlive) KeepAlive(token);

            return request_ctx;
        }




        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №1
        /// Добавляет пользовательский контекст с базовыми параметрами (token, userId, clientCode)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId">Id Web-пользователя</param>
        /// <param name="clientCode">доменное имя клиента</param>
        /// <param name="IsChangePasswordRequired">доменное имя клиента</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IContext Set(string token, string userId, string clientCode, bool IsChangePasswordRequired)
        {
            token = token.ToLower();

            if (Contains(token)) throw new ArgumentException();

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



        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №2
        /// Добавляет к существующему пользовательскому контексту доступные лицензии, указанную базу, профиль пользователя
        /// </summary>
        /// <param name="token">new server parameters</param>
        /// <param name="db">new server parameters</param>
        /// <param name="clientId">clientId</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void Set(string token, DatabaseModel db, int clientId)
        {
            token = token.ToLower();

            // Исключения отлавливает Application_Error в Global.asax

            if (!Contains(token)) throw new UserUnauthorized();

            var context = GetInternal(token);

            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            context.ClientLicence = dbProc.GetClientLicenceActive(clientId);

            db.ClientId = clientId;

            VerifyNumberOfConnectionsByNew(context, clientId, new List<DatabaseModel> { db });

            context.CurrentClientId = clientId;

            context.CurrentDB = db;

            var agentUser = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(context, context.CurrentEmployee.UserId);

            if (agentUser != null)
            {
                // проверка активности сотрудника
                if (!agentUser.IsActive)
                {
                    Remove(token);
                    throw new UserIsDeactivated(agentUser.Name);
                }

                if (agentUser.PositionExecutorsCount == 0)
                {
                    Remove(token);
                    throw new UserNotExecuteAnyPosition(agentUser.Name);
                }

                context.CurrentEmployee.AgentId = agentUser.AgentId;
                context.CurrentEmployee.Name = agentUser.Name;
                context.CurrentEmployee.LanguageId = agentUser.LanguageId;
            }
            else
            {
                Remove(token);
                throw new UserAccessIsDenied();
            }

            KeepAlive(token);

        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Добавляет к существующему пользовательскому контексту информации по логу
        /// </summary>
        /// <param name="token">new server parameters</param>
        /// <param name="loginLogId">clientId</param>
        /// <param name="loginLogInfo">clientId</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void Set(string token, int? loginLogId, string loginLogInfo)
        {
            token = token.ToLower();

            if (!Contains(token)) throw new UserUnauthorized();

            var context = GetInternal(token);

            context.LoginLogId = loginLogId;
            context.LoginLogInfo = loginLogInfo;

            KeepAlive(token);
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
            var context = GetInternal(token);
            context.CurrentPositionsIdList = positionsIdList;
            context.CurrentPositionsAccessLevel = DmsResolver.Current.Get<IAdminService>().GetCurrentPositionsAccessLevel(context);
            DmsResolver.Current.Get<IDictionaryService>().SetDictionaryAgentUserLastPositionChose(context, positionsIdList);
            // Контекст полностью сформирован и готов к работе
            context.IsFormed = true;
            KeepAlive(token);

            // Сохраняю текущий контекст
            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.SaveUserContexts(context);
        }

        /// <summary>
        /// Set client
        /// </summary>
        /// <param name="client">new client parameters</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void Set(FrontAspNetClient client)
        {
            string token = TokenLower;

            if (!Contains(token)) throw new UserUnauthorized();

            var context = GetInternal(token);

            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            context.ClientLicence = dbProc.GetClientLicenceActive(client.Id);

            var dbs = dbProc.GetServersByAdmin(new BL.Model.WebAPI.Filters.FilterAdminServers { ClientIds = new List<int> { client.Id } });

            VerifyNumberOfConnectionsByNew(context, client.Id, dbs);

            context.CurrentClientId = client.Id;

            // KeepAlive: Продление жизни пользовательского контекста
            KeepAlive(token);
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
            _casheContexts.Add(TokenLower, new StoreInfo() { StoreObject = val, LastUsage = DateTime.UtcNow });
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

            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();

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

        /// <summary>
        /// Удаляет пользовательский контекст из коллекции
        /// </summary>
        /// <returns>Typed setting value.</returns>
        public IContext Remove(string token = null)
        {
            if (string.IsNullOrEmpty(token)) token = TokenLower;

            if (!Contains(token)) return null;

            var ctx = GetInternal(token);
            var logger = DmsResolver.Current.Get<ILogger>();
            logger.UpdateLogDate1(ctx, ctx.LoginLogId.Value, _casheContexts[token].LastUsage);
            // удаляю пользовательский контекст из коллекции
            _casheContexts.Remove(token);

            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.DeleteUserContext(token);

            //HttpContext.Current.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return ctx;
        }

        /// <summary>
        /// Делает отметки в логе о последнем использовании контекста
        /// </summary>
        public void SaveLogContextsLastUsage()
        {
            var logger = DmsResolver.Current.Get<ILogger>();
            _casheContexts.Where(x => (x.Value.StoreObject is IContext) && ((IContext)x.Value.StoreObject).LoginLogId.HasValue).ToList()
            .ForEach(x =>
                {
                    var ctx = (x.Value.StoreObject as IContext);
                    logger.UpdateLogDate1(ctx, ctx.LoginLogId.Value, x.Value.LastUsage);
                });

        }

        /// <summary>
        /// Удаляет неиспользуемые пользовательские контексты
        /// </summary>
        public void RemoveByTimeout()
        {
            var now = DateTime.UtcNow;
            var keys = _casheContexts.Where(x => x.Value.LastUsage.AddMinutes(_TIME_OUT_MIN) <= now).Select(x => x.Key).ToArray();
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Удаляет пользовательские контексты по agentID
        /// </summary>
        /// <param name="agentId"></param>
        public void RemoveByAgentId(int agentId)
        {
            var now = DateTime.UtcNow;
            var keys = _casheContexts.Where(x => { try { return ((IContext)x.Value.StoreObject).CurrentAgentId == agentId; } catch { } return false; }).Select(x => x.Key).ToArray();
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Удаляет пользовательские контексты по userId
        /// </summary>
        /// <param name="userId"></param>
        public void RemoveByUserId(string userId)
        {
            var now = DateTime.UtcNow;
            var keys = _casheContexts.Where(x => { try { return ((IContext)x.Value.StoreObject).CurrentEmployee.UserId == userId; } catch { } return false; }).Select(x => x.Key).ToArray();
            foreach (var key in keys)
            {
                Remove(key);
            }
        }



        public void VerifyNumberOfConnectionsByNew(IContext context, int clientId, IEnumerable<DatabaseModel> dbs)
        {
            if (clientId <= 0) return;

            var si = new SystemInfo();

            var lic = context.ClientLicence;

            if (lic == null)
            {
                var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
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
        public void Clear()
        {
            _casheContexts.Clear();
        }

        private bool Contains(string token) => _casheContexts.ContainsKey(token);

        /// <summary>
        /// Количество активных пользователей
        /// </summary>
        public int Count
        {
            get { return _casheContexts.Count; }
        }

        private IContext GetInternal(string token)
        {
            var storeInfo = _casheContexts[token];

            try
            {
                return (IContext)storeInfo.StoreObject;
            }
            catch (InvalidCastException invalidCastException)
            {
                // TODO Это правильно, что при InvalidCastException выбрасывается new Exception()
                throw new Exception();
            }
        }

        private void KeepAlive(string token)
        {
            if (!_casheContexts.ContainsKey(token))
            {
                throw new UserUnauthorized();
            }

            var storeInfo = _casheContexts[token];
            // KeepAlive: Продление жизни пользовательского контекста
            storeInfo.LastUsage = DateTime.UtcNow;
        }

        public void Load()
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var list = webService.GetUserContexts(new BL.Model.WebAPI.Filters.FilterAspNetUserContext());

            foreach (var item in list)
            {
                var clientCode = webService.GetClientCode(item.ClientId);
                if (string.IsNullOrEmpty( clientCode)) continue;
                var server = webService.GetServerByUser(item.UserId, new SetUserServer { ClientId = item.ClientId, ServerId = -1 });
                if (server == null) continue;

                Set(item.Token, item.UserId, clientCode, item.IsChangePasswordRequired);
                Set(item.Token, server, item.ClientId);
                Set(item.Token, item.LoginLogId, item.LoginLogInfo);
                SetUserPositions(item.Token, item.CurrentPositionsIdList.Split(',').Select(n => Convert.ToInt32(n)).ToList());
            }
        }


    }
}
