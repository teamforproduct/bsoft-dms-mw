using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Database;
using BL.Model.Enums;
using Ninject;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.CrossCutting.Context;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.MailWorker
{
    public class MailService : IMailService, IDisposable
    {
        string _MAIL_SERVER_TYPE = "MAILSERVER_TYPE";
        string _MAIL_SERVER_NAME = "MAILSERVER_NAME";
        string _MAIL_SERVER_PORT = "MAILSERVER_PORT";
        string _MAIL_SERVER_LOGIN = "MAILSERVER_LOGIN";
        string _MAIL_SERVER_PASS = "MAILSERVER_PASSWORD";
        string _MAIL_SERVER_SYSTEMMAIL = "MAILSERVER_SYSTEMMAIL";
        string _MAIL_TIMEOUT_MIN = "MAILSERVER_TIMEOUT_MINUTE";

        private readonly ISystemDbProcess _sysDb;
        private readonly ISettings _settings;
        private readonly ILogger _logger;
        private object _lockObject;
        private readonly Dictionary<string, AdminContext> _serverContext;
        private Task _checkingThread;
        private readonly List<Timer> _timers;
        
        public MailService(ISystemDbProcess sysDb, ISettings settings, ILogger logger)
        {
            _sysDb = sysDb;
            _settings = settings;
            _logger = logger;
            _lockObject = new object();
            _serverContext = new Dictionary<string, AdminContext>();
            _timers = new List<Timer>();
        }

        private AdminContext GetAdminContext(string key)
        {
            AdminContext res = null;
            lock (_lockObject)
            {
                if (_serverContext.ContainsKey(key))
                    res = _serverContext[key];
            }
            return res;
        }

        public void Initialize(IEnumerable<DatabaseModel> dbList)
        {
            dbList.Select(x => new AdminContext(x)).ToList().ForEach(
                x=> _serverContext.Add(CommonSystemUtilities.GetServerKey(x),x));
            
            _checkingThread = Task.Factory.StartNew(InitializeServers);
        }

        private void InitializeServers()
        {
            foreach (var keyValuePair in _serverContext)
            {
                try
                {
                    var checkInterval = _settings.GetSetting<int>(keyValuePair.Value, _MAIL_TIMEOUT_MIN);

                    var msSetting = new InternalSendMailServerParameters
                    {
                        DatabaseKey = keyValuePair.Key,
                        ServerType = (MailServerType)_settings.GetSetting<int>(keyValuePair.Value, _MAIL_SERVER_TYPE),
                        FromAddress = _settings.GetSetting<string>(keyValuePair.Value, _MAIL_SERVER_SYSTEMMAIL),
                        Login = _settings.GetSetting<string>(keyValuePair.Value, _MAIL_SERVER_LOGIN),
                        Pass = _settings.GetSetting<string>(keyValuePair.Value, _MAIL_SERVER_PASS),
                        Server = _settings.GetSetting<string>(keyValuePair.Value, _MAIL_SERVER_NAME),
                        Port = _settings.GetSetting<int>(keyValuePair.Value, _MAIL_SERVER_PORT)
                    };

                    var tmr = new Timer(CheckForNewMessages, msSetting, 1000, checkInterval*1000);
                    _timers.Add(tmr);
                }
                catch (Exception ex)
                {
                    _logger.Error(keyValuePair.Value, "Could not start MeilSender for server", ex);
                }
            }
        }
        
        private void CheckForNewMessages(object state)
        {
            
            var md = state as InternalSendMailServerParameters;

            if (md == null) return;

            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            try
            {
                var processed = new InternalMailProcessed();
                var newEvents = _sysDb.GetNewActionsForMailing(ctx);
                foreach (var evt in newEvents)
                {
                    var mailParam = new InternalSendMailParameters(md);
                    //TODO make correct subject and body!
                    mailParam.ToAddress = evt.DestinationAgentEmail;
                    mailParam.Subject = "Automatic notification";
                    mailParam.Body = "You have new event: " + evt.Description;
                    SendMessage(ctx, mailParam);
                    processed.ProcessedEventIds.Add(evt.EventId);
                }
                _sysDb.MarkActionsLikeMailSended(ctx, processed);
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Error while processing new events and sending EMails", ex);
            }
        }


        private void SendMessage(IContext ctx, InternalSendMailParameters msSetting)
        {
            var sender = DmsResolver.Current.Kernel.Get<IMailSender>(msSetting.ServerType.ToString());

            if (sender == null) return;

            try
            {
                sender.SendMail(null, msSetting);
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Cannot send email!", msSetting, ex);
            }
        }

        public void Dispose()
        {
            _timers.ForEach(x =>
            {
                x.Change(Timeout.Infinite, Timeout.Infinite);
                x.Dispose();
            });
        }
    }
}