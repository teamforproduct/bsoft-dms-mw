using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Context;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Providers;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using WebGrease.Css.Extensions;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Коллекция пользовательских контекстов
    /// </summary>
    public class UserContexts
    {
        private readonly Dictionary<string, StoreInfo> _cacheContexts = new Dictionary<string, StoreInfo>();
        private const string _TOKEN_KEY = "Authorization";
        private const int _TIME_OUT_MIN = 15;
        private string Token { get { return HttpContext.Current.Request.Headers[_TOKEN_KEY]; } }
        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        private string TokenLower { get { return string.IsNullOrEmpty(Token) ? string.Empty : Token.ToLower(); } }


        public IQueryable<FrontSystemSession> GetContextListQuery()
        {
            locker.EnterReadLock();
            try
            {
                var res = _cacheContexts.AsQueryable()
                    .Where(x => x.Value.LastUsage > DateTime.UtcNow.AddMinutes(-1))
                    .Where(x => x.Value.StoreObject is IContext)
                    .Select(x => new FrontSystemSession
                    {
                        //Token = x.Key,
                        LastUsage = x.Value.LastUsage,
                        CreateDate = (x.Value.StoreObject as IContext).CreateDate,
                        LoginLogInfo = (x.Value.StoreObject as IContext).LoginLogInfo,
                        LoginLogId = (x.Value.StoreObject as IContext).LoginLogId,
                        UserId = (x.Value.StoreObject as IContext).Employee.UserId,
                        AgentId = (x.Value.StoreObject as IContext).Employee.AgentId,
                        Name = (x.Value.StoreObject as IContext).Employee.Name,
                        ClientId = (x.Value.StoreObject as IContext).Client.Id,
                        IsActive = true,
                        IsSuccess = true,
                    });
                return res;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <param name="currentPositionId"></param>
        /// <param name="isThrowExeception"></param>
        /// <param name="keepAlive"></param>
        /// <returns>Typed setting value.</returns>
        public IContext Get(int? currentPositionId = null, bool isThrowExeception = true, bool keepAlive = true, bool restoreToken = true)
        {
            string token = TokenLower;

            // пробую восстановить контекст из базы
            if (restoreToken && !Contains(token))
            {
                Restore(token);
            }

            if (!Contains(token)) throw new UserUnauthorized();

            var ctx = GetContextInternal(token);


            //TODO Licence
            //if (ctx.ClientLicence?.LicenceError != null)
            //{
            //    throw ctx.ClientLicence.LicenceError as DmsExceptions;
            //}

            //VerifyNumberOfConnections(ctx, ctx.Client.Id);

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
        /// <param name="userName">Id Web-пользователя</param>
        /// <param name="clientCode">доменное имя клиента</param>
        /// <param name="IsChangePasswordRequired">доменное имя клиента</param>
        /// <returns></returns>
        /// <exception cref="TokenAlreadyExists"></exception>
        public IContext Set(string token, string userId, string userName, bool IsChangePasswordRequired, string clientCode)
        {
            token = token.ToLower();

            if (Contains(token)) throw new TokenAlreadyExists();

            var dbWeb = DmsResolver.Current.Get<WebAPIDbProcess>();

            var context = new UserContext
            {
                Employee = new Employee
                {
                    Token = token,
                    UserId = userId,
                },
                Client = new Client
                {
                    Id = dbWeb.GetClientId(clientCode),
                    Code = clientCode
                },
            };


            context.IsChangePasswordRequired = IsChangePasswordRequired;
            context.UserName = userName;

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
        /// <returns></returns>
        public void Set(string token, DatabaseModelForAdminContext db)
        {
            token = token.ToLower();

            // Исключения отлавливает Application_Error в Global.asax

            var intContext = GetContextInternal(token);

            if (intContext == null) throw new UserUnauthorized();

            var dbWeb = DmsResolver.Current.Get<WebAPIDbProcess>();

            intContext.ClientLicence = dbWeb.GetClientLicenceActive(intContext.Client.Id);

            VerifyNumberOfConnectionsByNew(intContext, intContext.Client.Id, new List<DatabaseModelForAdminContext> { db });

            intContext.CurrentDB = db;

            var context = new UserContext(intContext);
            var agentUser = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(context, intContext.Employee.UserId);

            if (agentUser != null)
            {
                // эти проверки уже есть на получении токена
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

                intContext.Employee.AgentId = agentUser.AgentId;
                intContext.Employee.Name = agentUser.Name;
                intContext.Employee.LanguageId = agentUser.LanguageId;
            }
            else
            {
                Remove(token);
                throw new UserAccessIsDenied();
            }

            KeepAlive(token);

        }

        /// <summary>
        /// этап 3
        /// Формирование пользовательского контекста. 
        /// Добавляет к существующему пользовательскому контексту информации по логу
        /// </summary>
        /// <param name="token">new server parameters</param>
        /// <param name="browserInfo">clientId</param>
        /// <param name="fingerPrint">clientId</param>
        /// <returns></returns>
        public void Set(string token, string browserInfo, string fingerPrint)
        {
            token = token.ToLower();

            var intContext = GetContextInternal(token);

            if (intContext == null) throw new UserUnauthorized();

            var logger = DmsResolver.Current.Get<ILogger>();

            intContext.LoginLogInfo = browserInfo;
            var context = new UserContext(intContext);// это необходимо т.к. если несколько сервисов одновременно попытаются установить позиции для одного контекста, то возникнет ошибка. 
            intContext.LoginLogId = logger.Information(context, intContext.LoginLogInfo, (int)EnumObjects.System, (int)EnumSystemActions.Login, logDate: intContext.CreateDate, isCopyDate1: true);

            if (!string.IsNullOrEmpty(fingerPrint))
                logger.DeleteSystemLogs(intContext, new FilterSystemLog
                {
                    ObjectIDs = new List<int> { (int)EnumObjects.System },
                    ActionIDs = new List<int> { (int)EnumSystemActions.Login },
                    LogLevels = new List<int> { (int)EnumLogTypes.Error },
                    ExecutorAgentIDs = new List<int> { intContext.CurrentAgentId },
                    LogDateFrom = DateTime.UtcNow.AddMinutes(-60),
                    ObjectLog = $"\"FingerPrint\":\"{fingerPrint}\"",
                });

            KeepAlive(token);
        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №4
        /// Добавляет к существующему пользовательскому контексту список занимаемых должностей и AccessLevel
        /// </summary>
        /// <param name="token"></param>
        /// <param name="positionsIdList"></param>
        public void SetUserPositions(string token, List<int> positionsIdList)
        {
            var intContext = GetContextInternal(token);
            if (intContext == null) throw new UserUnauthorized();

            intContext.CurrentPositionsIdList = positionsIdList;

            var context = new UserContext(intContext);// это необходимо т.к. если несколько сервисов одновременно попытаются установить позиции для одного контекста, то возникнет ошибка. 
            intContext.CurrentPositionsAccessLevel = DmsResolver.Current.Get<IAdminService>().GetCurrentPositionsAccessLevel(context);
            context.CurrentPositionsAccessLevel = intContext.CurrentPositionsAccessLevel;

            DmsResolver.Current.Get<IDictionaryService>().SetDictionaryAgentUserLastPositionChose(context, positionsIdList);
            // Контекст полностью сформирован и готов к работе
            intContext.IsFormed = true;
            KeepAlive(token);

            // Сохраняю текущий контекст
            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.SaveUserContexts(intContext);
        }

        /// <summary>
        /// UpdateChangePasswordRequired
        /// </summary>
        /// <param name="isChangePasswordRequired"></param>
        /// <param name="userId">Id Web-пользователя</param>
        /// <returns></returns>
        public void UpdateChangePasswordRequired(string userId, bool isChangePasswordRequired)
        {
            locker.EnterReadLock();
            try
            {
                _cacheContexts.Select(x => x.Value.StoreObject as IContext)
                    .Where(x => x != null && x.Employee.UserId == userId)
                    .ForEach(x => x.IsChangePasswordRequired = isChangePasswordRequired);
            }
            catch (Exception ex)
            {
                //should not happen. add log here
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        private void Add(string token, IContext context)
        {
            // контекст добавленный в коллекцию считаю внутренним, его нельзя использовать для подключения к базе
            //context.IsInternal = true;
            _cacheContexts.Add(token.ToLower(), new StoreInfo() { StoreObject = context, LastUsage = DateTime.UtcNow });

        }

        private void Save(string token, IContext context)
        {
            locker.EnterWriteLock();
            try
            {
                Add(token, context);
            }
            finally
            {
                locker.ExitWriteLock();
            }

        }

        public void VerifyLicence(int clientId, IEnumerable<DatabaseModelForAdminContext> dbs)
        {
            IEnumerable<IContext> clientUsers = new List<IContext>();
            locker.EnterReadLock();
            try
            {
                clientUsers = _cacheContexts
                        .Select(x => (IContext)x.Value.StoreObject)
                        .Where(x => x.Client.Id == clientId);
            }
            finally
            {
                locker.ExitReadLock();
            }


            if (!clientUsers.Any())
                return;

            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();

            var lic = dbProc.GetClientLicenceActive(clientId);

            var si = new SystemInfo();
            var regCode = si.GetRegCode(lic);

            if (lic.IsConcurenteLicence)
            {
                locker.EnterReadLock();
                try
                {
                    var qry = _cacheContexts
                   .Select(x => (IContext)x.Value.StoreObject)
                   .Where(x => x.Client.Id == clientId)
                   .Select(x => x.Employee.UserId)
                   .Distinct();

                    lic.ConcurenteNumberOfConnectionsNow = qry.Count();
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }

            object licenceError;

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
        public IContext Remove(string token = null, bool removeFromBase = true)
        {
            if (string.IsNullOrEmpty(token)) token = TokenLower;


            var tknCtx = GetContextInternal(token);

            if (tknCtx == null) return null;

            var ctx = new UserContext(tknCtx);
            var logger = DmsResolver.Current.Get<ILogger>();
            // TODO  ctx.LoginLogId.Value - стреляется если нет LoginLogId
            logger.UpdateLogDate1(ctx, ctx.LoginLogId.Value, _cacheContexts[token].LastUsage);

            locker.EnterWriteLock();
            try
            {
                // удаляю пользовательский контекст из коллекции
                _cacheContexts.Remove(token);

                if (removeFromBase)
                {
                    //HttpContext.Current.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

                    var webService = DmsResolver.Current.Get<WebAPIService>();
                    webService.DeleteUserContext(token);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
            return ctx;
        }

        /// <summary>
        /// Делает отметки в логе о последнем использовании контекста
        /// </summary>
        public void SaveLogContextsLastUsage()
        {
            var logger = DmsResolver.Current.Get<ILogger>();
            locker.EnterReadLock();
            try
            {
                _cacheContexts.Where(x => (x.Value.StoreObject is IContext) && ((IContext)x.Value.StoreObject).LoginLogId.HasValue)
                    .ToList()
                    .ForEach(x =>
                    {
                        var ctx = (x.Value.StoreObject as IContext);
                        logger.UpdateLogDate1(ctx, ctx.LoginLogId.Value, x.Value.LastUsage);
                    });
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        /// <summary>
        /// Удаляет неиспользуемые пользовательские контексты
        /// </summary>
        public void RemoveByTimeout()
        {
            var now = DateTime.UtcNow;
            List<string> keys;
            locker.EnterReadLock();
            try
            {
                keys = _cacheContexts.Where(x => x.Value.LastUsage.AddMinutes(_TIME_OUT_MIN) <= now).Select(x => x.Key).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
            foreach (var key in keys)
            {
                // removeFromBase не убирать - это место исключение - его должно быть видно!!!
                Remove(key, false);
            }
        }

        /// <summary>
        /// Удаляет пользовательские контексты по agentID
        /// </summary>
        /// <param name="agentId"></param>
        public void RemoveByAgentId(int agentId)
        {
            List<string> keys;
            locker.EnterReadLock();
            try
            {
                keys = _cacheContexts.Where(x => x.Value.StoreObject is IContext && ((IContext)x.Value.StoreObject).CurrentAgentId == agentId).Select(x => x.Key).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
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
            List<string> keys;
            locker.EnterReadLock();
            try
            {
                keys = _cacheContexts.Where(x => x.Value.StoreObject is IContext && ((IContext)x.Value.StoreObject).Employee.UserId == userId).Select(x => x.Key).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
            foreach (var key in keys)
            {
                Remove(key);
            }
        }



        public void VerifyNumberOfConnectionsByNew(IContext context, int clientId, IEnumerable<DatabaseModelForAdminContext> dbs)
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
                var qry = new List<string>();
                locker.EnterReadLock();
                try
                {
                    qry = _cacheContexts
                    .Select(x => (IContext)x.Value.StoreObject)
                    .Where(x => x.Client.Id == clientId)
                    .Select(x => x.Employee.UserId)
                    .Distinct().ToList();
                }
                finally
                {
                    locker.ExitReadLock();
                }
                var count = qry.Count;

                if (qry.All(x => x != context.Employee.UserId))
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
                context.ClientLicence.LicenceError = licenceError;
            }
        }

        public void UpdateLanguageId(int agentId, int languageId)
        {
            List<IContext> contexts;
            locker.EnterReadLock();
            try
            {
                contexts = _cacheContexts
                        .Select(x => (IContext)x.Value.StoreObject)
                        .Where(x => x.Employee.AgentId == agentId).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
            foreach (var context in contexts)
            {
                context.Employee.LanguageId = languageId;
            }
        }

        /// <summary>
        /// Очистка всех пользовательских контекстов
        /// </summary>
        public void Clear()
        {
            locker.EnterWriteLock();
            try
            {
                _cacheContexts.Clear();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private bool Contains(string token)
        {
            locker.EnterReadLock();
            try
            {
                return _cacheContexts.ContainsKey(token);
            }
            finally
            {
                locker.ExitReadLock();
            }
        }


        /// <summary>
        /// Количество активных пользователей
        /// </summary>
        public int Count
        {
            get
            {
                locker.EnterReadLock();
                try
                {
                    return _cacheContexts.Count;
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }
        }


        private IContext GetContextInternal(string token)
        {
            locker.EnterReadLock();
            try
            {
                if (_cacheContexts.ContainsKey(token))
                {
                    var storeInfo = (IContext)_cacheContexts[token].StoreObject;
                    return storeInfo;
                }
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        private StoreInfo GetStoreInfoInternal(string token)
        {
            locker.EnterReadLock();
            try
            {
                if (_cacheContexts.ContainsKey(token))
                {
                    return _cacheContexts[token];
                }
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        private void KeepAlive(string token)
        {
            var storeInfo = GetStoreInfoInternal(token);
            if (storeInfo == null)
            {
                throw new UserUnauthorized();
            }

            // KeepAlive: Продление жизни пользовательского контекста
            storeInfo.LastUsage = DateTime.UtcNow;

            var ctx = (IContext)storeInfo.StoreObject;

            // не чаше, чем раз в 5 минут обновляю LastChangeDate
            if (ctx.LastChangeDate.AddMinutes(_TIME_OUT_MIN / 3) < DateTime.UtcNow)
            {
                ctx.LastChangeDate = DateTime.UtcNow;
                // Сохраняю текущий контекст
                var webService = DmsResolver.Current.Get<WebAPIService>();
                webService.UpdateUserContextLastChangeDate(token, ctx.LastChangeDate);
            }
        }

        private void Restore(string token)
        {
            locker.EnterWriteLock();
            try
            {
                // Попытка восстановить восстановленный контекст
                if (_cacheContexts.ContainsKey(token)) return;

                var webService = DmsResolver.Current.Get<WebAPIService>();
                var item = webService.GetUserContexts(new BL.Model.WebAPI.Filters.FilterAspNetUserContext()
                {
                    TokenExact = token
                }).FirstOrDefault();

                if (item == null) return;

                var clientCode = webService.GetClientCode(item.ClientId);

                if (string.IsNullOrEmpty(clientCode)) return;

                var server = webService.GetClientServer(item.ClientId);

                if (server == null) return;

                var user = webService.GetUserById(item.UserId);

                if (user == null) return;

                // Получаю информацию о браузере (она могла обновиться с момента предыдущего входа, например версия)
                var message = HttpContext.Current.Request.Browser.Info();

                // Тут нет никакого фингерпринта, он передается один раз с токеном
                var fingerPrint = HttpContext.Current.Request.InputStream.GetFingerprint();

                //Set(item.Token, item.UserId, user.UserName, user.IsChangePasswordRequired, clientCode);

                token = item.Token.ToLower();

                var dbWeb = DmsResolver.Current.Get<WebAPIDbProcess>();

                var context = new UserContext
                {
                    Employee = new Employee
                    {
                        Token = token,
                        UserId = item.UserId,
                    },
                    Client = new Client
                    {
                        Id = dbWeb.GetClientId(clientCode),
                        Code = clientCode
                    },
                    IsChangePasswordRequired = user.IsChangePasswordRequired,
                    UserName = user.UserName,
                };

                //Set(item.Token, server);

                context.ClientLicence = dbWeb.GetClientLicenceActive(context.Client.Id);

                VerifyNumberOfConnectionsByNew(context, context.Client.Id, new List<DatabaseModelForAdminContext> { server });//LONG

                context.CurrentDB = server;
                var dbCtx = DmsResolver.Current.Kernel.Get<DmsContext>(new ConstructorArgument("dbModel", context.CurrentDB));
                context.DbContext = dbCtx;
                var agentUser = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(context, context.Employee.UserId);

                if (agentUser != null)
                {
                    // эти проверки уже есть на получении токена
                    // проверка активности сотрудника
                    if (!agentUser.IsActive)
                    {
                        throw new UserIsDeactivated(agentUser.Name);
                    }

                    if (agentUser.PositionExecutorsCount == 0)
                    {
                        throw new UserNotExecuteAnyPosition(agentUser.Name);
                    }

                    context.Employee.AgentId = agentUser.AgentId;
                    context.Employee.Name = agentUser.Name;
                    context.Employee.LanguageId = agentUser.LanguageId;
                }
                else
                {
                    throw new UserAccessIsDenied();
                }

                //Set(item.Token, message, fingerPrint);

                var logger = DmsResolver.Current.Get<ILogger>();

                context.LoginLogInfo = message;
                context.LoginLogId = logger.Information(context, context.LoginLogInfo, (int)EnumObjects.System, (int)EnumSystemActions.Login, logDate: context.CreateDate, isCopyDate1: true);

                if (!string.IsNullOrEmpty(fingerPrint))
                    logger.DeleteSystemLogs(context, new FilterSystemLog
                    {
                        ObjectIDs = new List<int> { (int)EnumObjects.System },
                        ActionIDs = new List<int> { (int)EnumSystemActions.Login },
                        LogLevels = new List<int> { (int)EnumLogTypes.Error },
                        ExecutorAgentIDs = new List<int> { context.CurrentAgentId },
                        LogDateFrom = DateTime.UtcNow.AddMinutes(-60),
                        ObjectLog = $"\"FingerPrint\":\"{fingerPrint}\"",
                    });

                //SetUserPositions(item.Token,item.CurrentPositionsIdList.Split(',').Select(n => Convert.ToInt32(n)).ToList());
                var positionsIdList = item.CurrentPositionsIdList.Split(',').Select(n => Convert.ToInt32(n)).ToList();

                context.CurrentPositionsIdList = positionsIdList;
                context.CurrentPositionsAccessLevel = DmsResolver.Current.Get<IAdminService>().GetCurrentPositionsAccessLevel(context);
                DmsResolver.Current.Get<IDictionaryService>().SetDictionaryAgentUserLastPositionChose(context, positionsIdList);
                // Контекст полностью сформирован и готов к работе
                context.IsFormed = true;

                // Сохраняю текущий контекст
                webService.SaveUserContexts(context);//TODO надоли это если мы только что отресторили токен? - дата проставляется свежая

                context.DbContext = null;
                dbCtx.Dispose();

                Add(token, context);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

    }
}
