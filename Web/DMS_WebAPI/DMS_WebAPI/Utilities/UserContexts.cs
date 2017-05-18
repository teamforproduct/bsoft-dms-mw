using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Context;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Providers;
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
                        UserId = (x.Value.StoreObject as IContext).User.Id,
                        AgentId = (x.Value.StoreObject as IContext).Employee.Id,
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
            if (isThrowExeception && request_ctx.User.IsChangePasswordRequired)
                throw new UserMustChangePassword();

            // KeepAlive: Продление жизни пользовательского контекста
            if (keepAlive) KeepAlive(token);

            return request_ctx;
        }




        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №1
        /// Добавляет пользовательский контекст с базовыми параметрами (token, userId, clientCode)
        /// Добавляет к существующему пользовательскому контексту доступные лицензии, указанную базу, профиль пользователя
        /// Добавляет к существующему пользовательскому контексту информации по логу
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId">Id Web-пользователя</param>
        /// <param name="userName">Id Web-пользователя</param>
        /// <param name="clientCode">доменное имя клиента</param>
        /// <param name="IsChangePasswordRequired">доменное имя клиента</param>
        /// <param name="db">new server parameters</param>
        /// <param name="browserInfo">clientId</param>
        /// <param name="fingerPrint">clientId</param>
        /// <returns></returns>
        /// <exception cref="TokenAlreadyExists"></exception>
        public IContext Set(string token, string userId, string userName, bool IsChangePasswordRequired, string clientCode, DatabaseModelForAdminContext db, string browserInfo, string fingerPrint)
        {
            token = token.ToLower();

            if (Contains(token)) throw new TokenAlreadyExists();

            var intContext = GetContextInternal(token, userId, userName, IsChangePasswordRequired, clientCode, db, browserInfo, fingerPrint);

            // TODO вернуть, когда перейдем к лицензиям
            //VerifyNumberOfConnectionsByNew(intContext, new List<DatabaseModelForAdminContext> { db });

            // запись в лог создает новый дб контекст, поэтому передаю internal
            WriteLog(intContext);

            AddWithLock(token, intContext);

            return intContext;

        }

        

        private IContext GetContextInternal(string token, string userId, string userName, bool IsChangePasswordRequired, string clientCode, DatabaseModelForAdminContext db, string browserInfo, string fingerPrint)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var clientId = webService.GetClientId(clientCode);

            #region LoginLogInfo

            var logInfo = browserInfo;

            if (!string.IsNullOrEmpty(fingerPrint))
            {
                var fp = webService.GetUserFingerprints(new FilterAspNetUserFingerprint { UserIDs = new List<string> { userId }, FingerprintExact = fingerPrint }).FirstOrDefault();

                if (fp != null)
                {
                    logInfo = $"{logInfo};{fp.Fingerprint};{fp.Name}";
                }
                else
                {
                    logInfo = $"{logInfo};{fingerPrint.TruncateHard(8)}...;Not Saved";
                }
            }
            #endregion

            var intContext = new UserContext
            {
                Token = token,
                User = new User
                {
                    Id = userId,
                    Name = userName,
                    Fingerprint = fingerPrint,
                    IsChangePasswordRequired = IsChangePasswordRequired,
                    LanguageId = -1,
                },
                Employee = new Employee { },
                Client = new Client
                {
                    Id = clientId,
                    Code = clientCode
                },
                
                CurrentDB = db,
                ClientLicence = webService.GetClientLicenceActive(clientId),
                LoginLogInfo = logInfo,
            };

            var context = new UserContext(intContext);
            var agentUser = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(context, userId);

            if (agentUser != null)
            {
                // проверка активности сотрудника
                if (!agentUser.IsActive)
                {
                    throw new EmployeeIsDeactivated(agentUser.Name);
                }

                if (agentUser.PositionExecutorsCount == 0)
                {
                    throw new EmployeeNotExecuteAnyPosition(agentUser.Name);
                }

                intContext.Employee.Id = agentUser.Id;
                intContext.Employee.Name = agentUser.Name;
                intContext.Employee.LanguageId = agentUser.LanguageId;
                intContext.User.LanguageId = agentUser.LanguageId;
            }
            else
            {
                throw new UserAccessIsDenied();
            }

            return intContext;
        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №2
        /// Добавляет к существующему пользовательскому контексту список занимаемых должностей и AccessLevel
        /// </summary>
        /// <param name="token"></param>
        /// <param name="positionsIdList"></param>
        public void SetUserPositions(string token, List<int> positionsIdList)
        {
            var intContext = GetContextInternal(token);
            if (intContext == null) throw new UserUnauthorized();

            SetUserPositions(intContext, positionsIdList);

            SaveInBase(intContext);

            KeepAlive(token);
        }

        private void SetUserPositions(IContext intContext, List<int> positionsIdList)
        {
            intContext.CurrentPositionsIdList = positionsIdList;

            var context = new UserContext(intContext);// это необходимо т.к. если несколько сервисов одновременно попытаются установить позиции для одного контекста, то возникнет ошибка. 

            intContext.CurrentPositionsAccessLevel = DmsResolver.Current.Get<IAdminService>().GetCurrentPositionsAccessLevel(context);
            context.CurrentPositionsAccessLevel = intContext.CurrentPositionsAccessLevel;

            DmsResolver.Current.Get<IDictionaryService>().SetDictionaryAgentUserLastPositionChose(context, positionsIdList);
            // Контекст полностью сформирован и готов к работе
            intContext.IsFormed = true;
        }

        private void SaveInBase(IContext intContext)
        {
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
                    .Where(x => x != null && x.User.Id == userId)
                    .ForEach(x => x.User.IsChangePasswordRequired = isChangePasswordRequired);
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

        private void AddWithLock(string token, IContext context)
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
                   .Select(x => x.User.Id)
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
                keys = _cacheContexts.Where(x => x.Value.StoreObject is IContext && ((IContext)x.Value.StoreObject).User.Id == userId).Select(x => x.Key).ToList();
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



        public void VerifyNumberOfConnectionsByNew(IContext context, IEnumerable<DatabaseModelForAdminContext> dbs)
        {
            if (context.Client.Id <= 0) return;

            var si = new SystemInfo();

            var lic = context.ClientLicence;

            if (lic == null)
            {
                var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
                context.ClientLicence = lic = dbProc.GetClientLicenceActive(context.Client.Id);
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
                    .Where(x => x.Client.Id == context.Client.Id)
                    .Select(x => x.User.Id)
                    .Distinct().ToList();
                }
                finally
                {
                    locker.ExitReadLock();
                }
                var count = qry.Count;

                if (qry.All(x => x != context.User.Id))
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
                        .Where(x => x.Employee.Id == agentId).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
            foreach (var context in contexts)
            {
                context.Employee.LanguageId = languageId;
                context.User.LanguageId = languageId;
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
                var item = webService.GetUserContexts(new FilterAspNetUserContext()
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

                var brInfo = HttpContext.Current.Request.Browser.Info();

                var intContext = GetContextInternal(item.Token, user.Id, user.UserName, user.IsChangePasswordRequired, clientCode,
                     server, brInfo, item.Fingerprint);

                // TODO вернуть, когда перейдем к лицензиям 
                //VerifyNumberOfConnectionsByNew(context, new List<DatabaseModelForAdminContext> { server });//LONG

                WriteLog(intContext);


                SetUserPositions(intContext, item.CurrentPositionsIdList.Split(',').Select(n => Convert.ToInt32(n)).ToList());

                //TODO надоли это если мы только что отресторили токен? - Да, дата проставляется свежая
                SaveInBase(intContext);

                Add(token, intContext);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private void WriteLog(IContext context)
        {
            var logger = DmsResolver.Current.Get<ILogger>();
            context.LoginLogId = logger.Information(context, context.LoginLogInfo, (int)EnumObjects.System, (int)EnumSystemActions.Login, logDate: context.CreateDate, isCopyDate1: true);

            // не понятно чего это такое
            if (!string.IsNullOrEmpty(context.User.Fingerprint))
                logger.DeleteSystemLogs(context, new FilterSystemLog
                {
                    ObjectIDs = new List<int> { (int)EnumObjects.System },
                    ActionIDs = new List<int> { (int)EnumSystemActions.Login },
                    LogLevels = new List<int> { (int)EnumLogTypes.Error },
                    ExecutorAgentIDs = new List<int> { context.CurrentAgentId },
                    LogDateFrom = DateTime.UtcNow.AddMinutes(-60),
                    ObjectLog = $"\"FingerPrint\":\"{context.User.Fingerprint}\"",
                });

        }

    }
}
