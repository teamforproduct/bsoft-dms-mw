using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Extensions;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.AdminCore.Clients;
using BL.Model.AdminCore.WebUser;
using BL.Model.Common;
using BL.Model.Context;
using BL.Model.DictionaryCore.FrontModel.Employees;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DatabaseContext;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Infrastructure;
using DMS_WebAPI.Models;
using DMS_WebAPI.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BL.Logic.SystemServices.TaskManagerService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIService
    {
        public int GetClientId(string clientCode)
        {
            return _webDb.GetClientId(clientCode);
        }

        public string GetClientCode(int clientId)
        {
            return _webDb.GetClientCode(clientId);

        }

        public DatabaseModelForAdminContext GetClientServer(int clientId)
        {
            return _webDb.GetClientServer(clientId);
        }

        public DatabaseModelForAdminContext GetClientServer(string clientCode)
        {
            return _webDb.GetClientServer(clientCode);
        }

        public bool ExistsClients(FilterAspNetClients filter)
        {
            return _webDb.ExistsClients(filter);
        }

        public async Task<string> AddClientByEmail(AddClientFromHash model)
        {
            var responseString = string.Empty;

            var request = _webDb.GetClientRequests(new FilterAspNetClientRequests { HashCodeExact = model.Hash }).FirstOrDefault();

            if (request == null) throw new ClientRequestIsNotFound();

            request.Password = model.Password;
            request.OrgName = model.OrgName;
            request.DepartmentName = model.DepartmentName;
            request.PositionName = model.PositionName;

            var isDone = await AddClientSaaS(request);

            if (isDone) _webDb.DeleteClientRequest(new FilterAspNetClientRequests { HashCodeExact = model.Hash });

            //---------------------------------------------------

            // есть задача авторизовать по темному нового пользователя

            if (!string.IsNullOrEmpty(model.Password))
            {
                var setVal = DmsResolver.Current.Get<ISettingValues>();
                var localHost = setVal.GetLocalHost();
                var uri = new Uri(new Uri(localHost), ApiPrefix.V3 + "token");

                var values = new Dictionary<string, string>
                {
                   { "username", request.Email },
                   { "password", model.Password },
                   { "client_id", request.ClientCode },
                   { "client_secret", request.ClientCode },
                   { "scope", "" },
                   { "grant_type", "password" },
                   { "fingerprint", "SystemAuthorization" }
                };

                var content = new FormUrlEncodedContent(values);

                var httpContext = HttpContext.Current;
                var httpClient = DmsResolver.Current.Get<HttpClient>();

                httpClient.DefaultRequestHeaders.Clear();
                // Начиняю Headers запроса параметрами из текущего запроса
                foreach (var key in httpContext.Request.Headers.AllKeys)
                {

                    if (key == "Content-Type") continue;
                    if (key == "Content-Length") continue;

                    try
                    {
                        var value = httpContext.Request.Headers.Get(key);
                        //content.Headers.Add(key, value);
                        httpClient.DefaultRequestHeaders.Add(key, value);
                    }
                    catch { }

                }

                var response = await httpClient.PostAsync(uri, content);

                responseString = await response.Content.ReadAsStringAsync();

            }

            return responseString;
        }

        public async Task AddClientBySMS(AddClientFromSMS model)
        {
            var request = _webDb.GetClientRequests(new FilterAspNetClientRequests { SMSCodeExact = model.SMSCode }).FirstOrDefault();

            if (request == null) throw new ClientRequestIsNotFound();

            var isDone = await AddClientSaaS(request);

            if (isDone) _webDb.DeleteClientRequest(new FilterAspNetClientRequests { SMSCodeExact = model.SMSCode });
        }

        public async Task<bool> AddClientSaaS(AddClientSaaS model)
        {
            // Проверка уникальности доменного имени
            if (_webDb.ExistsClients(new FilterAspNetClients { Code = model.ClientCode })) throw new ClientCodeAlreadyExists(model.ClientCode);

            var httpClient = DmsResolver.Current.Get<HttpClient>();

            var tmpService = DmsResolver.Current.Get<ISettingValues>();
            var mHost = tmpService.GetMainHost();
            var vHost = tmpService.GetVirtualHost();
#if DEBUG
            vHost = "http://10.88.12.21:82";
#endif
            var request = $"{vHost}/newhost.pl?fqdn={model.ClientCode}.{mHost}";

            var responseString = await httpClient.GetStringAsync(request);

            switch (responseString)
            {
                case "Created":
                    break;
                case "BadRequest":
                    throw new ClientCodeRequired(); //- не указан параметр fqdn
                case "PreconditionFailed":
                    throw new ClientCodeInvalid(); //- параметр не соответствует маске[-0 - 9a - z].ostrean.com
                case "Conflict":
                    throw new ClientCodeAlreadyExists(model.ClientCode); //- такой субдомен уже существует
                default:
                    throw new ClientCreateException(new List<string> { responseString });
            }

            // сервер может определяться более сложным образом: с учетом нагрузки, количества клиентов
            var settings = DmsResolver.Current.Get<ISettingValues>();
            var dbName = settings.GetCurrentServerName();
            var languages = DmsResolver.Current.Get<ILanguages>();

            // Если не указан язык, беру язык по умолчанию 
            var languageId = string.IsNullOrEmpty(model.Language)
                ? languages.GetLanguageIdByHttpContext()
                : languages.GetLanguageIdByCode(model.Language);

            var db = _webDb.GetServers(new FilterAdminServers { ServerNameExact = dbName }).FirstOrDefault();

            if (db == null)
            {
                // определяю сервер для клиента пока первый попавшийся
                db = _webDb.GetServers(new FilterAdminServers()).FirstOrDefault();
            }

            if (db == null) throw new ServerIsNotFound();


            //if (string.IsNullOrEmpty(model.Password)) model.Password = "admin_" + model.ClientCode;

            using (var transaction = Transactions.GetTransaction())
            {
                // Создаю запись о клиенте
                model.ClientId = _webDb.AddClient(new ModifyAspNetClient
                {
                    Name = model.ClientCode,
                    Code = model.ClientCode,
                    LanguageId = languageId,
                });


                // Линкую клиента на сервер
                _webDb.AddClientServer(new SetClientServer
                {
                    ClientId = model.ClientId,
                    ServerId = db.Id,
                });


                transaction.Complete();
            }

            var dbAdmin = new DatabaseModelForAdminContext(db);
            dbAdmin.ClientId = model.ClientId;
            dbAdmin.ClientCode = model.ClientCode;

            var ctx = new AdminContext(dbAdmin);
            ctx.User.LanguageId = languageId;


            var clientService = DmsResolver.Current.Get<IClientService>();


            try
            {
                // Предзаполняю клиентскую базу настроками, ролями
                clientService.AddDictionary(ctx, model);

            }
            catch (Exception)
            {
                if (model.ClientId > 0) await DeleteClient(model.ClientId);

                throw;
            }

            AddUserEmployeeInOrg(ctx,
                new AddEmployeeInOrg
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    OrgName = model.OrgName, // languages.GetTranslation(ctx.Employee.LanguageId, "##l@Clients:" + "MyCompany" + "@l##"),
                    DepartmentIndex = "01",
                    DepartmentName = model.DepartmentName, // languages.GetTranslation(ctx.Employee.LanguageId, "##l@Clients:" + "MyDepartment" + "@l##"),
                    PositionName = model.PositionName, // languages.GetTranslation(ctx.Employee.LanguageId, "##l@Clients:" + "MyPosition" + "@l##"),
                    ExecutorType = EnumPositionExecutionTypes.Personal,
                    AccessLevel = EnumAccessLevels.Personally,
                    LanguageId = ctx.User.LanguageId,
                    Phone = model.PhoneNumber,
                    Login = model.Email,
                    Role = Roles.Admin,
                    Password = model.Password,
                    // Создание клиента происходит по факту клика по ссылке в письме, поэтому при создании пользователя подтверждать емаил не нужно
                    EmailConfirmed = true,
                    IsChangePasswordRequired = false,
                    IsEmailConfirmRequired = false,
                },
                new AddJournalsInOrg
                {
                    IncomingJournalIndex = "01",
                    IncomingJournalName = languages.GetTranslation(ctx.User.LanguageId, languages.GetLabel("Journals", EnumDocumentDirections.Incoming.ToString())),

                    OutcomingJournalIndex = "02",
                    OutcomingJournalName = languages.GetTranslation(ctx.User.LanguageId, languages.GetLabel("Journals", EnumDocumentDirections.Outcoming.ToString())),

                    InternalJournalIndex = "03",
                    InternalJournalName = languages.GetTranslation(ctx.User.LanguageId, languages.GetLabel("Journals", EnumDocumentDirections.Internal.ToString())),
                },
                sendEmail: false);

            try
            {
                //add workers for new client. Check if settings exists for that workers. 
                var tskInit = DmsResolver.Current.Get<ICommonTaskInitializer>();
                tskInit.InitWorkersForClient(dbAdmin);
            }
            catch (Exception)
            {
                if (model.ClientId > 0) await DeleteClient(model.ClientId);

                throw;
            }

            //UserManager.AddLogin(userId, new UserLoginInfo {    })
            return true;

        }

        public async Task DeleteClient(int Id)
        {
            if (Id == 1) throw new ClientIsNotFound();

            var client = _webDb.GetClients(new FilterAspNetClients { IDs = new List<int> { Id } }).FirstOrDefault();

            if (client == null) throw new ClientIsNotFound();

            var taskManager = DmsResolver.Current.Get<ICommonTaskInitializer>();
            taskManager.RemoveWorkersForClient(Id);

            var server = _webDb.GetClientServer(Id);
            var ctx = new AdminContext(server);
            var clientService = DmsResolver.Current.Get<IClientService>();
            clientService.Delete(ctx);

            var clients = new List<int> { Id };
            //using (var transaction = Transactions.GetTransaction())
            {
                _webDb.DeleteClientLicence(new FilterAspNetClientLicences { ClientIds = clients });

                DeleteUsersInClient(Id, null);

                _webDb.DeleteClientServer(new FilterAspNetClientServer { ClientIDs = clients });

                _webDb.DeleteClient(Id);

                //transaction.Complete();
            }
            var httpClient = DmsResolver.Current.Get<HttpClient>();

            var tmpService = DmsResolver.Current.Get<ISettingValues>();
            var mHost = tmpService.GetMainHost();
            var vHost = tmpService.GetVirtualHost();
            var request = $"{vHost}/deletehost.pl?fqdn={client.Code}.{mHost}";
            var responseString = await httpClient.GetStringAsync(request);

        }

        public int AddClient(AddAspNetClient model)
        {
            //TODO в transaction не может подлючиться к базе
            if (model.Server.Id <= 0)
            {
                _webDb.InitializerDatabase(model.Server);
            }

            // Проверка уникальности доменного имени
            if (_webDb.ExistsClients(new FilterAspNetClients { Code = model.Client.Code })) throw new ClientCodeAlreadyExists(model.Client.Code);

            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));


                #region Create client 1 

                // Создаю клиента
                var clientId = _webDb.AddClient(new ModifyAspNetClient
                {
                    Name = model.Client.Name,
                    Code = model.Client.Code,
                });

                AspNetClientLicences clientLicence;
                if (model.LicenceId > 0)
                {
                    _webDb.AddClientLicence(clientId, model.LicenceId.GetValueOrDefault());
                }

                #endregion Create client 1

                transaction.Complete();

                return clientId;

            }
        }

        public bool ExistsClient(FilterAspNetClients filter)
        {
            var setVals = DmsResolver.Current.Get<ISettingValues>();
            var banHosts = setVals.GetSystemHosts();

            if (filter == null) throw new FilterRequired();

            if (banHosts.Contains(filter.Code)) return true;

            var f = new FilterAspNetClientRequests { CodeExact = filter.Code };

            // Проверка уникальности доменного имени
            var exists = ExistsClientRequest(f);

            if (exists) return true;

            return _webDb.ExistsClients(filter);
        }

        public bool ExistsClientRequest(FilterAspNetClientRequests filter)
        {
            if (filter == null) throw new FilterRequired();

            return _webDb.ExistsClientRequests(filter);
        }

        public IEnumerable<FrontAspNetClientRequest> GetClientRequest(FilterAspNetClientRequests filter)
        {
            return _webDb.GetClientRequests(filter);
        }

        public async Task<int> AddClientSaaSRequest(AddClientSaaS model)
        {
            if (!string.IsNullOrEmpty(model.Recaptcha))
            {
                //  https://developers.google.com/recaptcha/docs/verify
                var setVal = DmsResolver.Current.Get<ISettingValues>();
                var url = setVal.GetGoogleReCaptchaURL();

                var values = new Dictionary<string, string>
                {
                   { "secret", setVal.GetGoogleReCaptchaSecret() }, // Required. The shared key between your site and reCAPTCHA.
                   { "response", model.Recaptcha }, // Required. The user response token provided by reCAPTCHA, verifying the user on your site.
                   //{ "remoteip",  }, // Optional. The user's IP address.
                };

                var content = new FormUrlEncodedContent(values);

                var httpClient = DmsResolver.Current.Get<HttpClient>();

                var response = await httpClient.PostAsync(url, content);

                /* The response is a JSON object:
                {
                  "success": true|false,
                  "challenge_ts": timestamp,  // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
                  "hostname": string,         // the hostname of the site where the reCAPTCHA was solved
                  "error-codes": [...]        // optional
                } */

                //if (response.StatusCode != System.Net.HttpStatusCode.OK) throw new HttpException((int)response.StatusCode);

                var json = await response.Content.ReadAsStringAsync();

                var jObject = JObject.Parse(json);

                if (jObject.Value<bool>("success") == false)
                {
                    throw new ClientCreateException(jObject.GetValue("error-codes").Select(x => x.ToString()).ToList());
                }

            }

            if (ExistsClient(new FilterAspNetClients { Code = model.ClientCode })) throw new ClientCodeAlreadyExists(model.ClientCode);

            if (string.IsNullOrEmpty(model.ClientName)) model.ClientName = model.ClientCode;

            int res = 0;

            var languages = DmsResolver.Current.Get<ILanguages>();

            try
            {
                model.HashCode = model.ClientCode.md5();
                model.SMSCode = DateTime.UtcNow.ToString("ssHHmm");

                if (string.IsNullOrEmpty(model.Language))
                {
                    var language = languages.GetDefaultLanguage();
                    if (language != null) model.Language = language.Code;
                }

                res = _webDb.AddClientRequest(model);

                var tmpService = DmsResolver.Current.Get<ISettingValues>();
                var addr = tmpService.GetAuthAddress();
                var callbackurl = new Uri(new Uri(addr), "new-client").AbsoluteUri;

                // isNew можно вычислить только на текущий момент времени (пользователь может сделать несколько компаний)
                var isNew = !ExistsUser(model.Email);

                callbackurl += $"?hash={model.HashCode}&login={model.Email}&code={model.ClientCode}&isNew={isNew}&language={model.Language}";

                var m = new NewCompanyRequestModel()
                {
                    Url = callbackurl,
                    ClientCode = model.ClientCode,
                    ClientName = model.ClientName,
                    FirstName = model.FirstName,
                    Language = model.Language
                };

                var htmlContent = m.RenderPartialViewToString(RenderPartialView.NewCompanyRequest);
                var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();


                mailService.SendMessage(null, MailServers.Docum, model.Email, languages.GetTranslation("##l@Mail.Welcome.Subject@l##"), htmlContent);
            }
            catch (Exception)
            {
                if (res > 0) _webDb.DeleteClientRequest(new FilterAspNetClientRequests { IDs = new List<int> { res } });
                throw;
            }

            return res;
        }

        public void DeleteOldClientRequest()
        {
            // Заявки которые старее 24 часов
            _webDb.DeleteClientRequest(new FilterAspNetClientRequests { DateCreateLess = DateTime.UtcNow.AddDays(-1) });
        }

        public void DeleteOldUserContexts()
        {
            // контексты, которые не испольвались 14 дней
            _webDb.DeleteUserContexts(new FilterAspNetUserContext { LastUsegeDateLess = DateTime.UtcNow.AddDays(-14) });
        }

    }
}