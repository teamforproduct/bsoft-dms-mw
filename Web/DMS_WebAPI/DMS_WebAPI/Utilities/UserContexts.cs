using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Context;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.WebAPI.Filters;
using DMS_WebAPI.DBModel;
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
    public class UserContexts //: ICollection<UserContext>
    {
        private readonly List<DefaultContext> _cacheContexts = new List<DefaultContext>();
        private const string _TOKEN_KEY = "Authorization";
        private const string _HOST = "Host";
        private const int _TIME_OUT_MIN = 15;
        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        private string CurrentKey
        {
            get
            {
                var t = HttpContext.Current.Request.Headers[_TOKEN_KEY];
                return string.IsNullOrEmpty(t) ? string.Empty : t.ToLower().md5();
            }
        }
        private string GetClientCode()
        {
            var host = HttpContext.Current.Request.Headers[_HOST];

            if (host.Contains("localhost")) return "docum";

            // TODO определить доменное имя 3 уровня
            return host.Substring(0, host.IndexOf('.'));
        }

        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <param name="currentPositionId"></param>
        /// <param name="AsIs"></param>
        /// <returns>Typed setting value.</returns>
        public IContext Get(int? currentPositionId = null, bool AsIs = false)
        {
            string key = CurrentKey;

            // пробую восстановить контекст из базы
            if (!AsIs && !Contains(key))
            {
                Restore(key);
            }

            if (!Contains(key)) throw new UserUnauthorized();

            var intContext = GetContextInternal(key);

            if (!AsIs && string.IsNullOrEmpty(intContext.Client.Code))
            {
                // Тут нужно проерить нет ли приглашений пользователя и добавить линку клиент-пользователь
                SetClientParms(intContext, GetClientCode());
                SaveInBase(intContext);
            };

            //TODO Licence
            //if (ctx.ClientLicence?.LicenceError != null)
            //{
            //    throw ctx.ClientLicence.LicenceError as DmsExceptions;
            //}

            //VerifyNumberOfConnections(ctx, ctx.Client.Id);

            var ctx = new DefaultContext(intContext);

            if (!AsIs) ctx.SetCurrentPosition(currentPositionId);
            if (!AsIs && ctx.User.IsChangePasswordRequired) throw new UserMustChangePassword();

            // KeepAlive: Продление жизни пользовательского контекста
            KeepAlive(key);

            return ctx;
        }


        /// <summary>
        /// Gets setting value by its name.
        /// </summary>
        /// <param name="AsIs"></param>
        /// <returns>Typed setting value.</returns>
        public IAuthContext GetAuthContext( bool AsIs = false)
        {
            string key = CurrentKey;

            // пробую восстановить контекст из базы
            if (!AsIs && !Contains(key))
            {
                Restore(key);
            }

            if (!Contains(key)) throw new UserUnauthorized();

            var intContext = GetContextInternal(key);

            var ctx = new AuthContext(intContext);

            if (!AsIs && ctx.User.IsChangePasswordRequired) throw new UserMustChangePassword();

            // KeepAlive: Продление жизни пользовательского контекста
            KeepAlive(key);

            return ctx;
        }


        private DefaultContext GetContextInternal(string key)
        {
            DefaultContext res = null;
            locker.EnterReadLock();
            try
            {
                res = _cacheContexts.Where(x => x.Key == key).FirstOrDefault();
            }
            finally
            {
                locker.ExitReadLock();
            }
            return res;
        }

        private void Add(DefaultContext context, bool withLocker = true)
        {
            if (withLocker) locker.EnterWriteLock();
            try
            {
                // контекст добавленный в коллекцию считаю внутренним, его нельзя использовать для подключения к базе
                _cacheContexts.Add(context);
            }
            finally
            {
                if (withLocker) locker.ExitWriteLock();
            }

        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №1
        /// Добавляет пользовательский контекст с базовыми параметрами (token, userId, clientCode)
        /// Добавляет к существующему пользовательскому контексту доступные лицензии, указанную базу, профиль пользователя
        /// Добавляет к существующему пользовательскому контексту информации по логу
        /// </summary>
        /// <param name="key"></param>
        /// <param name="user">Id Web-пользователя</param>
        /// <param name="clientCode">доменное имя клиента</param>
        /// <param name="db">new server parameters</param>
        /// <param name="model">SessionEnviroment</param>
        /// <returns></returns>
        /// <exception cref="TokenAlreadyExists"></exception>
        public DefaultContext Add(AspNetUsers user, SessionEnviroment model)
        {
            var key = model.Session;

            if (Contains(key)) throw new TokenAlreadyExists();

            var intContext = FormContextInternal(key, user);

            // TODO вернуть, когда перейдем к лицензиям
            //VerifyNumberOfConnectionsByNew(intContext, new List<DatabaseModelForAdminContext> { db });

            Add(intContext);

            WriteSessionLog(intContext, model);
            SaveInBase(intContext);

            return intContext;

        }

        private DefaultContext FormContextInternal(string key, AspNetUsers user)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();

            var intContext = new DefaultContext
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
                Session = new Session { },
                Client = new Client { },
            };

            return intContext;
        }


        /// <summary>
        /// Удаляет пользовательский контекст из коллекции
        /// </summary>
        /// <returns>Typed setting value.</returns>
        public bool Remove(string key = null, bool removeFromBase = true)
        {
            if (string.IsNullOrEmpty(key)) key = CurrentKey;

            var intContext = GetContextInternal(key);

            if (intContext == null) return false;

            locker.EnterWriteLock();
            try
            {
                var webService = DmsResolver.Current.Get<WebAPIService>();

                var fingerprint = webService.GetSessionLogFingerprint(intContext.Session.SignInId);

                var log = new AddSessionLog
                {
                    UserId = intContext.User.Id,
                    Type = EnumLogTypes.Information,
                    Event = removeFromBase ? "SessionStop" : "SessionSuspend",
                    Message = removeFromBase ? "SessionStopped" : "SessionSuspended",
                    Date = intContext.Session.LastUsage,

                    Session = key,//HttpContext.Current.Request.Browser.Identifier(),
                    Platform = HttpContext.Current.Request.Browser.Platform,
                    Browser = HttpContext.Current.Request.Browser.Name(),
                    IP = HttpContext.Current.Request.Browser.IP(),
                    Fingerprint = fingerprint,
                };

                intContext = null;

                // удаляю пользовательский контекст из коллекции
                _cacheContexts.RemoveAll(x => x.Key == key);

                webService.AddSessionLog(log);

                if (removeFromBase)
                {
                    // TODO нужно овиновскую сессию тоже останавливать
                    //HttpContext.Current.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                    webService.DeleteUserContext(key);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }

            return true;
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
                keys = _cacheContexts.Where(x => x.Session.LastUsage.AddMinutes(_TIME_OUT_MIN) <= now).Select(x => x.Key).ToList();
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
        /// Удаляет пользовательские контексты по clientId
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        public void RemoveByClientId(int clientId, string userId)
        {
            List<string> keys;
            locker.EnterReadLock();
            try
            {
                keys = _cacheContexts.Where(x => x.User.Id == userId && x.Client.Id == clientId).Select(x => x.Key).ToList();
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
            foreach (var key in keys)
            {
                Remove(key);
            }
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


            foreach (var key in keys)
            {
                Remove(key);
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

        private bool Contains(string key, bool withLocker = true)
        {
            var res = false;
            if (withLocker) locker.EnterReadLock();
            try
            {
                res = _cacheContexts.Any(x => x.Key == key);
            }
            finally
            { if (withLocker) locker.ExitReadLock(); }
            return res;
        }


        /// <summary>
        /// Количество активных пользователей
        /// </summary>
        public int Count
        {
            get
            {
                var res = 0;
                locker.EnterReadLock();
                try
                {
                    res = _cacheContexts.Count;
                }
                finally
                { locker.ExitReadLock(); }
                return res;
            }
        }




        private void KeepAlive(string key)
        {
            var intContext = GetContextInternal(key);
            if (intContext == null)
            {
                throw new UserUnauthorized();
            }

            // KeepAlive: Продление жизни пользовательского контекста
            intContext.Session.LastUsage = DateTime.UtcNow;

            //// не чаше, чем раз в 5 минут обновляю LastChangeDate
            //if (intContext.Session.LastUpdate.AddMinutes(_TIME_OUT_MIN / 3) < DateTime.UtcNow)
            //{
            //    intContext.Session.LastUpdate = DateTime.UtcNow;
            //    // Сохраняю текущий контекст
            //    var webService = DmsResolver.Current.Get<WebAPIService>();
            //    webService.UpdateUserContextLastChangeDate(key, intContext.Session.LastUpdate);
            //}
        }

        private void SetClientParms(DefaultContext intContext, string clientCode)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();

            var clientId = webService.GetClientId(clientCode);

            if (clientId <= 0) throw new ClientIsNotFound();

            // Проверяю принадлежность пользователя к клиенту
            if (!webService.ExistsUserInClientByUserId(intContext.User.Id, clientId)) throw new ClientIsNotContainUser(clientCode);

            var server = webService.GetClientServer(clientCode);
            if (server == null) throw new DatabaseIsNotFound();

            // TODO вернуть, когда перейдем к лицензиям
            //VerifyNumberOfConnectionsByNew(intContext, new List<DatabaseModelForAdminContext> { db });

            intContext.Client.Id = clientId;
            intContext.Client.Code = clientCode;

            intContext.CurrentDB = server;

            intContext.ClientLicence = webService.GetClientLicenceActive(clientId);


            var context = new DefaultContext(intContext);
            var employee = DmsResolver.Current.Get<IAdminService>().GetEmployeeForContext(context, intContext.User.Id);

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

        }

        private void Restore(string key)
        {
            locker.EnterWriteLock();
            try
            {
                // Попытка восстановить восстановленный контекст
                if (Contains(key, false)) return;

                var webService = DmsResolver.Current.Get<WebAPIService>();
                var item = webService.GetUserContexts(new FilterAspNetUserContext()
                {
                    Key = key
                }).FirstOrDefault();

                if (item == null) return;

                var user = webService.GetUserById(item.UserId);

                if (user == null) return;

                var intContext = FormContextInternal(item.Key, user);


                if (item.ClientId > 0)
                {
                    var clientCode = webService.GetClientCode(item.ClientId);
                    SetClientParms(intContext, clientCode);

                    // TODO вернуть, когда перейдем к лицензиям 
                    //VerifyNumberOfConnectionsByNew(context, new List<DatabaseModelForAdminContext> { server });//LONG

                    if (!string.IsNullOrEmpty(item.CurrentPositionsIdList))
                    {
                        SetUserPositions(intContext, item.CurrentPositionsIdList.Split(',').Select(n => Convert.ToInt32(n)).ToList());
                    }
                }

                Add(intContext, false);

                var fingerprint = string.Empty;
                if (item.SignInId.HasValue) fingerprint = webService.GetSessionLogFingerprint(item.SignInId.Value);

                var s = new SessionEnviroment
                {
                    Session = key,
                    Browser = HttpContext.Current.Request.Browser.Name(),
                    IP = HttpContext.Current.Request.Browser.IP(),
                    Platform = HttpContext.Current.Request.Browser.Platform,
                    // Fingerprint вычитываю из предыдущего лога, так как на фронте он повторно не расчитывается
                    Fingerprint = fingerprint
                };

                WriteSessionLog(intContext, s, true);

                //TODO надоли это если мы только что отресторили токен? - Да, дата проставляется свежая
                //SaveInBase(intContext);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Формирование пользовательского контекста. 
        /// Этап №2
        /// Добавляет к существующему пользовательскому контексту список занимаемых должностей и AccessLevel
        /// </summary>
        /// <param name="positionsIdList"></param>
        public void SetUserPositions(List<int> positionsIdList)
        {
            var key = CurrentKey;

            var intContext = GetContextInternal(key);
            if (intContext == null) throw new UserUnauthorized();

            SetUserPositions(intContext, positionsIdList);

            SaveInBase(intContext);

            KeepAlive(key);
        }

        private void SetUserPositions(IContext intContext, List<int> positionsIdList)
        {
            intContext.CurrentPositionsIdList = positionsIdList;

            var context = new DefaultContext(intContext);// это необходимо т.к. если несколько сервисов одновременно попытаются установить позиции для одного контекста, то возникнет ошибка. 

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
            locker.ExitWriteLock();
            try
            {
                _cacheContexts
                    .Where(x => x != null && x.User.Id == userId)
                    .ForEach(x => x.User.IsChangePasswordRequired = isChangePasswordRequired);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private void WriteSessionLog(DefaultContext context, SessionEnviroment model, bool SessionRestore = false)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();

            var finger = string.Empty;

            if (!string.IsNullOrEmpty(model.Fingerprint))
            {
                var fp = webService.GetUserFingerprints(new FilterAspNetUserFingerprint { UserIDs = new List<string> { context.User.Id }, FingerprintExact = model.Fingerprint }).FirstOrDefault();

                if (fp != null)
                {
                    finger = fp.Name;
                }
                else
                {
                    finger = model.Fingerprint.TruncateHard(8);
                }
            }

            var log = new AddSessionLog
            {
                Type = EnumLogTypes.Information,
                Event = SessionRestore ? "SessionRestore" : "SessionStart",
                Message = SessionRestore ? "SessionRestored" : "SessionStarted",
                Date = context.Session.CreateDate,
                LastUsage = context.Session.LastUsage,

                UserId = context.User.Id,

                Session = model.Session,
                Fingerprint = finger,
                Browser = model.Browser,
                Platform = model.Platform,
                IP = model.IP,
            };

            context.Session.SignInId = webService.AddSessionLog(log);

            // При успешном входе в систему деактивирую записи об угадывании ответа на сектерный вопрос
            // Свои логи видит только пользователь поэтому безсмысленно чегото писать чтобы потом удалить
            //var e = new UserAnswerIsIncorrect();

            //webService.SetSessionLogEnabled(false, new FilterSessionsLog
            //{
            //    Message = e.GetType().Name,
            //    UserId = context.User.Id,
            //    Types = new List<EnumLogTypes> { EnumLogTypes.Error },
            //    DateFrom = DateTime.UtcNow.AddMinutes(-60),
            //    IPExact = model.IP
            //});

            //new UserAnswerIsIncorrect(), user.Id, EnumLogTypes.Error,

            // не понятно чего это такое
            //if (!string.IsNullOrEmpty(context.User.Fingerprint))
            //    logger.DeleteSystemLogs(context, new FilterSystemLog
            //    {
            //        ObjectIDs = new List<int> { (int)EnumObjects.System },
            //        ActionIDs = new List<int> { (int)EnumActions.Login },
            //        LogLevels = new List<int> { (int)EnumLogTypes.Error },
            //        ExecutorAgentIDs = new List<int> { context.CurrentAgentId },
            //        LogDateFrom = DateTime.UtcNow.AddMinutes(-60),
            //        ObjectLog = $"\"FingerPrint\":\"{context.User.Fingerprint}\"",
            //    });

        }

        public void VerifyLicence(int clientId, IEnumerable<DatabaseModelForAdminContext> dbs)
        {
            IEnumerable<IContext> clientUsers = new List<IContext>();
            locker.EnterReadLock();
            try
            {
                clientUsers = _cacheContexts
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
            List<DefaultContext> contexts;
            locker.EnterReadLock();
            try
            {
                contexts = _cacheContexts.Where(x => x.User.Id == userId).ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }

            contexts.ForEach(x => { x.User.LanguageId = languageId; });

        }

        /// <summary>
        /// Делает отметки в логе о последнем использовании контекста
        /// </summary>
        public void SaveLogContextsLastUsage()
        {
            List<DefaultContext> contexts;
            locker.EnterReadLock();
            try
            {
                var time = DateTime.UtcNow.AddMinutes(-1);
                contexts = _cacheContexts.Where(x => x.Session.SignInId > 0 && x.Session.LastUsage > time).ToList();

            }
            finally
            {
                locker.ExitReadLock();
            }

            if (contexts.Any())
            {
                var sessionIds = contexts.Select(x => x.Session.SignInId).ToList();

                var webService = DmsResolver.Current.Get<WebAPIService>();
                webService.SetSessionLogLastUsage(DateTime.UtcNow, new FilterSessionsLog { IDs = sessionIds });
            }
        }

        public IEnumerable<int> GetActiveAgentsList(int clientId)
        {
            locker.EnterReadLock();
            try
            {
                var time = DateTime.UtcNow.AddMinutes(-1);
                var res = _cacheContexts
                    .Where(x => x.Employee != null && x.Client != null && x.Client.Id == clientId && x.Session.LastUsage > time)
                    .Select(x => x.Employee.Id).Distinct().ToList();
                return res;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

    }
}
