using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Database;
using BL.Model.Enums;
using BL.Model.SystemCore;
using Ninject;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.CrossCutting.Context;



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

        private Dictionary<Timer, SendMailData> _serverSettings; 
        private readonly ISettings _settings;
        private readonly ILogger _logger;
        private object _lockObject;
        List<AdminContext> _serverContext;
        Task _checkingThread;

        private void AddMailData(Timer key, SendMailData data)
        {
            lock (_lockObject)
            {
                if (_serverSettings.ContainsKey(key))
                {
                    _serverSettings[key] = data;
                }
                else
                {
                    _serverSettings.Add(key, data);
                }
            }
        }

        private SendMailData GetMailData(Timer key)
        {
            SendMailData res = null;
            lock (_lockObject)
            {
                if (_serverSettings.ContainsKey(key))
                {
                    res = _serverSettings[key];
                }
            }
            return res;
        }

        public MailService(ISettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
            _serverSettings = new Dictionary<Timer, SendMailData>();
        }

        public void Initialize(IEnumerable<DatabaseModel> dbList)
        {
            _serverContext = dbList.Select(x => new AdminContext(x)).ToList();
            _checkingThread = Task.Factory.StartNew(InitializeServers);
        }

        private void InitializeServers()
        {
            foreach (var ctx in _serverContext)
            {
                try
                {
                    var checkInterval = _settings.GetSetting<int>(ctx, _MAIL_TIMEOUT_MIN);

                    var msSetting = new SendMailData
                    {
                        ServerType = _settings.GetSetting<MailServerType>(ctx, _MAIL_SERVER_TYPE),
                        FromAddress = _settings.GetSetting<string>(ctx, _MAIL_SERVER_SYSTEMMAIL),
                        Login = _settings.GetSetting<string>(ctx, _MAIL_SERVER_LOGIN),
                        Pass = _settings.GetSetting<string>(ctx, _MAIL_SERVER_PASS),
                        Server = _settings.GetSetting<string>(ctx, _MAIL_SERVER_NAME),
                        Port = _settings.GetSetting<int>(ctx, _MAIL_SERVER_PORT)
                    };

                    var key = new Timer(CheckForNewMessages, msSetting, 1000, checkInterval*1000);
                    AddMailData(key, msSetting);
                }
                catch (Exception ex)
                {
                    _logger.Error(ctx, "Could not start MeilSender for server", ex);
                }
            }
        }



        private void CheckForNewMessages(object state)
        {
            Timer currTimer = state as Timer;
            if (currTimer != null)
            {
                var param = GetMailData(currTimer);

            }
            var md = state as SendMailData;
            if (md != null)
            {
            }
        }


        private void SendMessage(SendMailData msSetting)
        {

            var sender = DmsResolver.Current.Kernel.Get<IMailSender>(msSetting.ServerType.ToString());
            if (sender != null)
            {
                var sendData = new SendMailData(msSetting);

                //sender.SendMail(sendData);
            }
        }

        public void Dispose()
        {
            
        }
    }
}