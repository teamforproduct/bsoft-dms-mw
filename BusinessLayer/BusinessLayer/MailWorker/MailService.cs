using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.Common;
using BL.Logic.DependencyInjection;
using BL.Model.Database;
using BL.Model.Enums;
using BL.Model.SystemCore;
using Ninject;
using System.Linq;
using BL.CrossCutting.Context;

namespace BL.Logic.MailWorker
{
    public class MailService : IMailService
    {
        string _MAIL_SERVER_TYPE = "MAILSERVER_TYPE";
        string _MAIL_SERVER_NAME = "MAILSERVER_NAME";
        string _MAIL_SERVER_PORT = "MAILSERVER_PORT";
        string _MAIL_SERVER_LOGIN = "MAILSERVER_LOGIN";
        string _MAIL_SERVER_PASS = "MAILSERVER_PASSWORD";
        string _MAIL_SERVER_SYSTEMMAIL = "MAILSERVER_SYSTEMMAIL";
        string _MAIL_TIMEOUT_MIN = "MAILSERVER_TIMEOUT_MINUTE";

        private Dictionary<string, SendMailData> _serverSettings; 
        private readonly ISettings _settings;
        List<AdminContext> _serverContext;


        public MailService(ISettings settings)
        {
            _settings = settings;
            _serverSettings = new Dictionary<string, SendMailData>();
        }

        public void Initialize(IEnumerable<DatabaseModel> dbList)
        {
            _serverContext = dbList.Select(x => new AdminContext(x)).ToList();
            
        }

        private void SendMessage(IContext ctx)
        {
            var key = CommonSystemUtilities.GetServerKey(ctx);
            SendMailData msSetting;
            if (_serverSettings.ContainsKey(key))
            {
                msSetting = _serverSettings[key];
            }
            else
            {
                msSetting = new SendMailData
                {
                    ServerType = _settings.GetSetting<MailServerType>(ctx, _MAIL_SERVER_TYPE),
                    FromAddress = _settings.GetSetting<string>(ctx, _MAIL_SERVER_SYSTEMMAIL),
                    Login = _settings.GetSetting<string>(ctx, _MAIL_SERVER_LOGIN),
                    Pass = _settings.GetSetting<string>(ctx, _MAIL_SERVER_PASS),
                    Server = _settings.GetSetting<string>(ctx, _MAIL_SERVER_NAME),
                    Port = _settings.GetSetting<int>(ctx, _MAIL_SERVER_PORT)
                };
                _serverSettings.Add(key,msSetting);
            }


            var sender = DmsResolver.Current.Kernel.Get<IMailSender>(msSetting.ServerType.ToString());
            if (sender != null)
            {
                var sendData = new SendMailData(msSetting);

                //sender.SendMail(sendData);
            }
        }
    }
}