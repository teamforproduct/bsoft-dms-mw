using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Context;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore.Filters;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Providers;
using Microsoft.AspNet.Identity;
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
        private readonly List<IContext> _cacheContexts = new List<IContext>();
        private const string _TOKEN_KEY = "Authorization";
        private const string _HOST = "Host";
        private const int _TIME_OUT_MIN = 15;
        private string CurrentKey { get { return HttpContext.Current.Request.Headers[_TOKEN_KEY].md5(); } }
        private string ClientCode
        {
            get
            {
                var host = HttpContext.Current.Request.Headers[_HOST];

                if (host.Contains("localhost"))
                {
                    return "docum";
                }

                var uri = new Uri(host);

                // TODO определить доменное имя 3 уровня
                return uri.Host.Substring(0, host.IndexOf('.'));
            }
        }
        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public IQueryable<FrontSystemSession> GetContextListQuery()
        {
            locker.EnterReadLock();
            try
            {
                var res = _cacheContexts.AsQueryable()
                    .Where(x => x.LastUsage > DateTime.UtcNow.AddMinutes(-1))
                    .Select(x => new FrontSystemSession
                    {
                        LastUsage = x.LastUsage,
                        CreateDate = x.CreateDate,
                        LoginLogId = x.LoginLogId,
                        UserId = x.User.Id,
                        AgentId = x.Employee.Id,
                        Name = x.Employee.Name,
                        ClientId = x.Client.Id,
                        IsActive = true,
                        IsSuccess = true,
                        Host = "_.ostrean.com"
                    });
                return res;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="currentPositionId"></param>
        /// <param name="isThrowExeception"></param>
        /// <param name="keepAlive"></param>
        /// <returns>Typed setting value.</returns>
        public IContext Get(int? currentPositionId = null, bool isThrowExeception = true, bool keepAlive = true, bool restoreToken = true)
        {
            var key = CurrentKey;

            // пробую восстановить контекст из базы
            if (restoreToken && !Contains(key))
            {
                Restore(key);

                if (!Contains(key))
                {
                    Create(key);
                }
            }

            if (!Contains(key)) throw new UserUnauthorized();

            var ctx = GetInternal(key);


            //TODO Licence
            //if (ctx.ClientLicence?.LicenceError != null)
            //{
            //    throw ctx.ClientLicence.LicenceError as DmsExceptions;
            //}

            //VerifyNumberOfConnections(ctx, ctx.Client.Id);

            var request_ctx = new UserContext(ctx);
            request_ctx.SetCurrentPosition(currentPositionId);
            if (isThrowExeception && request_ctx.User.IsChangePasswordRequired) throw new UserMustChangePassword();

            // KeepAlive: Продление жизни пользовательского контекста
            if (keepAlive) KeepAlive(key);

            return request_ctx;
        }

        private void Create(string key)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();

            var clientId = webService.GetClientId(ClientCode);

            if (clientId <= 0) throw new ClientIsNotFound();

            var user = webService.GetUserById(HttpContext.Current.User.Identity.GetUserId());
            if (user == null) throw new UserIsNotDefined();


            // Тут нужно проерить нет ли приглашений пользователя и добавить линку клиент-пользователь

            // Проверяю принадлежность пользователя к клиенту
            if (!webService.ExistsUserInClient(user, clientId)) throw new ClientIsNotContainUser(ClientCode);

            var server = webService.GetClientServer(ClientCode);
            if (server == null) throw new DatabaseIsNotFound();

            // Получаю информацию о браузере
            //var brInfo = HttpContext.Current.Request.Browser.Info();

            //var fingerPrint = context.Request.Body.GetFingerprintAsync();

            //#region Подпорка для соапа
            //if (string.IsNullOrEmpty(fingerPrint))
            //{
            //    var scope = await context.Request.Body.GetScopeAsync();

            //    if (scope == "fingerprint") fingerPrint = "SoapUI finger";

            //}
            //#endregion

            var intContext = FormContext(key, user, ClientCode, server);

            // TODO вернуть, когда перейдем к лицензиям
            //VerifyNumberOfConnectionsByNew(intContext, new List<DatabaseModelForAdminContext> { db });

            // запись в лог создает новый дб контекст, поэтому передаю internal
            WriteLog(intContext);

            AddWithLock(intContext);

        }



        private IContext FormContext(string key, AspNetUsers user, string clientCode, DatabaseModelForAdminContext db)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var clientId = webService.GetClientId(clientCode);

            var intContext = new UserContext
            {
                Key = key,
                User = new User
                {
                    Id = user.Id,
                    Name = user.UserName,
                    IsChangePasswordRequired = user.IsChangePasswordRequired,
                    LanguageId = user.LanguageId,
                },
                Employee = new Employee { },
                Client = new Client
                {
                    Id = clientId,
                    Code = clientCode
                },

                CurrentDB = db,
                ClientLicence = webService.GetClientLicenceActive(clientId),
            };

            var context = new UserContext(intContext);
            var employee = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(context, user.Id);

            if (employee != null)
            {
                if (employee.IsLockout) throw new EmployeeIsLockoutByAdmin(employee.Name);

                // проверка активности сотрудника
                if (!employee.IsActive) throw new EmployeeIsDeactivated(employee.Name);


                if (employee.AssigmentsCount == 0) throw new EmployeeNotExecuteAnyPosition(employee.Name);

                intContext.Employee.Id = employee.Id;
                intContext.Employee.Name = employee.Name;
            }
            else
            {
                throw new EmployeeIsNotDefined();
            }

            return intContext;
        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №2
        /// Добавляет к существующему пользовательскому контексту список занимаемых должностей и AccessLevel
        /// </summary>
        /// <param name="key"></param>
        /// <param name="positionsIdList"></param>
        public void SetUserPositions(string key, List<int> positionsIdList)
        {
            var intContext = GetInternal(key);
            if (intContext == null) throw new UserUnauthorized();

            SetUserPositions(intContext, positionsIdList);

            SaveInBase(intContext);

            KeepAlive(key);
        }

        private void SetUserPositions(IContext intContext, List<int> positionsIdList)
        {
            intContext.CurrentPositionsIdList = positionsIdList;

            var context = new UserContext(intContext);// это необходимо т.к. если несколько сервисов одновременно попытаются установить позиции для одного контекста, то возникнет ошибка. 

            intContext.CurrentPositionsAccessLevel = DmsResolver.Current.Get<IAdminService>().GetCurrentPositionsAccessLevel(context);
            context.CurrentPositionsAccessLevel = intContext.CurrentPositionsAccessLevel;

            DmsResolver.Current.Get<IDictionaryService>().SetAgentUserLastPositionChose(context, context.Employee.Id, positionsIdList);


            // Контекст полностью сформирован и готов к работе
            intContext.IsFormed = true;
        }

        private void SaveInBase(IContext intContext)
        {
            // Сохраняю текущий контекст
            var webService = DmsResolver.Current.Get<WebAPIService>();
            intContext.LastChangeDate = DateTime.UtcNow;
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
                _cacheContexts.Where(x => x != null && x.User.Id == userId)
                    .ForEach(x => x.User.IsChangePasswordRequired = isChangePasswordRequired);
            }
            catch (Exception)
            {
                //should not happen. add log here
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        private void Add(IContext context)
        {
            context.LastUsage = DateTime.UtcNow;
            _cacheContexts.Add(context);

        }

        private void AddWithLock(IContext context)
        {
            locker.EnterWriteLock();
            try
            {
                Add(context);
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
                clientUsers = _cacheContexts.Where(x => x.Client.Id == clientId);
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

        public void Remove(List<string> keys, bool removeFromBase = true)
        {
            if (keys == null || !keys.Any()) return;

            var intContexts = GetInternals(keys);

            if (!intContexts.Any()) return;

            //var logger = DmsResolver.Current.Get<ILogger>();
            //logger.UpdateLogDate1(ctx, intContext.LoginLogId.Value, intContext.LastUsage);

            locker.EnterWriteLock();

            try
            {
                // удаляю пользовательский контекст из коллекции
                _cacheContexts.RemoveAll(x => keys.Contains(x.Key));

                if (removeFromBase)
                {
                    // TODO нужно овиновскую сессию тоже останавливать
                    //HttpContext.Current.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

                    var webService = DmsResolver.Current.Get<WebAPIService>();
                    webService.DeleteUserContexts(new FilterAspNetUserContext { KeyExact = keys });
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }


        }

        /// <summary>
        /// Удаляет пользовательский контекст из коллекции
        /// </summary>
        /// <returns>Typed setting value.</returns>
        public void Remove(string key = null, bool removeFromBase = true)
        {
            if (string.IsNullOrEmpty(key)) key = CurrentKey;

            var intContext = GetInternal(key);

            if (intContext == null) return;

            var ctx = new UserContext(intContext);
            var logger = DmsResolver.Current.Get<ILogger>();
            // TODO  ctx.LoginLogId.Value - стреляется если нет LoginLogId
            logger.UpdateLogDate1(ctx, intContext.LoginLogId.Value, intContext.LastUsage);

            locker.EnterWriteLock();

            try
            {
                // удаляю пользовательский контекст из коллекции
                _cacheContexts.RemoveAll(x => x.Key == key);

                if (removeFromBase)
                {
                    // TODO нужно овиновскую сессию тоже останавливать
                    //HttpContext.Current.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

                    var webService = DmsResolver.Current.Get<WebAPIService>();
                    webService.DeleteUserContexts(new FilterAspNetUserContext { KeyExact = new List<string> { key } });
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
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
                _cacheContexts.Where(x => x.LoginLogId.HasValue)
                    .ForEach(x =>
                    {
                        logger.UpdateLogDate1(x, x.LoginLogId.Value, x.LastUsage);
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
            List<IContext> ctxs;
            locker.EnterReadLock();
            try
            {
                ctxs = _cacheContexts.Where(x => x.LastUsage.AddMinutes(_TIME_OUT_MIN) <= now).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
            // removeFromBase не убирать - это место исключение - его должно быть видно!!!
            Remove(ctxs.Select(x => x.Key).ToList(), removeFromBase: false);
        }

        /// <summary>
        /// Удаляет пользовательские контексты по clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="employeeId"></param>
        public void RemoveByClientId(int clientId, int employeeId)
        {
            List<string> keys;
            locker.EnterReadLock();
            try
            {
                keys = _cacheContexts.Where(x => x.Employee.Id == employeeId && x.Client.Id == clientId).Select(x => x.Key).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }

            Remove(keys);
        }

        /// <summary>
        /// Удаляет пользовательские контексты по userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="exceptCurrent"></param>
        public void RemoveByUserId(string userId, bool exceptCurrent = false)
        {
            List<string> keys;
            locker.EnterReadLock();
            try
            {
                var qry = _cacheContexts.Where(x => x.User.Id == userId);

                if (exceptCurrent)
                {
                    qry = qry.Where(x => x.Key != CurrentKey);
                }

                keys = qry.Select(x => x.Key).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }

            Remove(keys);
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

        public void UpdateLanguageId(string userId, int languageId)
        {
            List<IContext> contexts;
            locker.EnterReadLock();
            try
            {
                contexts = _cacheContexts
                        .Where(x => x.User.Id == userId).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }

            contexts.ForEach(x => { x.User.LanguageId = languageId; });

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

        private bool Contains(string key)
        {
            locker.EnterReadLock();
            try
            {
                return _cacheContexts.Any(x => x.Key == key);
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


        private IContext GetInternal(string key)
        {
            locker.EnterReadLock();
            try
            {
                if (Contains(key))
                {
                    return _cacheContexts.Where(x => x.Key == key).FirstOrDefault();
                }
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        private List<IContext> GetInternals(List<string> keys)
        {
            locker.EnterReadLock();
            try
            {
                return _cacheContexts.Where(x => keys.Contains(x.Key)).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        private void KeepAlive(string key)
        {
            var ctx = GetInternal(key);
            if (ctx == null) throw new UserUnauthorized();

            // KeepAlive: Продление жизни пользовательского контекста
            ctx.LastUsage = DateTime.UtcNow;

            // не чаше, чем раз в 5 минут обновляю LastChangeDate
            if (ctx.LastChangeDate.AddMinutes(_TIME_OUT_MIN / 3) < DateTime.UtcNow)
            {
                ctx.LastChangeDate = DateTime.UtcNow;
                // Сохраняю текущий контекст
                var webService = DmsResolver.Current.Get<WebAPIService>();
                webService.UpdateUserContextLastChangeDate(key, ctx.LastChangeDate);
            }
        }

        private void Restore(string key)
        {
            locker.EnterWriteLock();
            try
            {
                // Попытка восстановить восстановленный контекст
                if (Contains(key)) return;

                var webService = DmsResolver.Current.Get<WebAPIService>();
                var item = webService.GetUserContexts(new FilterAspNetUserContext()
                {
                    KeyExact = new List<string> { key },
                }).FirstOrDefault();

                if (item == null) return;

                var clientCode = webService.GetClientCode(item.ClientId);

                if (string.IsNullOrEmpty(clientCode)) return;

                var server = webService.GetClientServer(item.ClientId);

                if (server == null) return;

                var user = webService.GetUserById(item.UserId);

                if (user == null) return;

                var brInfo = HttpContext.Current.Request.Browser.Info();

                var intContext = FormContext(key, user, clientCode, server);

                // TODO вернуть, когда перейдем к лицензиям 
                //VerifyNumberOfConnectionsByNew(context, new List<DatabaseModelForAdminContext> { server });//LONG


                // TODO 
                //var log = item.LogId;

                //#region LoginLogInfo

                //var logInfo = browserInfo;

                //if (!string.IsNullOrEmpty(fingerPrint))
                //{
                //    var fp = webService.GetUserFingerprints(new FilterAspNetUserFingerprint { UserIDs = new List<string> { user.Id }, FingerprintExact = fingerPrint }).FirstOrDefault();

                //    if (fp != null)
                //    {
                //        logInfo = $"{logInfo};{fp.Fingerprint};{fp.Name}";
                //    }
                //    else
                //    {
                //        logInfo = $"{logInfo};{fingerPrint.TruncateHard(8)}...;Not Saved";
                //    }
                //}
                //#endregion

                WriteLog(intContext);


                SetUserPositions(intContext, item.CurrentPositionsIdList.Split(',').Select(n => Convert.ToInt32(n)).ToList());

                //TODO надоли это если мы только что отресторили токен? - Да, дата проставляется свежая
                SaveInBase(intContext);

                Add(intContext);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private void WriteLog(IContext context)
        {
            //var logger = DmsResolver.Current.Get<ILogger>();
            //context.LoginLogId = logger.Information(context, context.LoginLogInfo, (int)EnumObjects.System, (int)EnumSystemActions.Login, logDate: context.CreateDate, isCopyDate1: true);

            //// не понятно чего это такое
            //if (!string.IsNullOrEmpty(context.User.Fingerprint))
            //    logger.DeleteSystemLogs(context, new FilterSystemLog
            //    {
            //        ObjectIDs = new List<int> { (int)EnumObjects.System },
            //        ActionIDs = new List<int> { (int)EnumSystemActions.Login },
            //        LogLevels = new List<int> { (int)EnumLogTypes.Error },
            //        ExecutorAgentIDs = new List<int> { context.CurrentAgentId },
            //        LogDateFrom = DateTime.UtcNow.AddMinutes(-60),
            //        ObjectLog = $"\"FingerPrint\":\"{context.User.Fingerprint}\"",
            //    });

        }

    }
}
