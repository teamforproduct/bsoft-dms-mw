using System;
using System.Collections.Generic;
using System.Threading;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DependencyInjection;
using BL.Model.Constants;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using Ninject;

namespace BL.Logic.SystemServices.MailWorker
{
    public class MailSenderWorkerService : BaseSystemWorkerService
    {
        private readonly ISystemDbProcess _sysDb;

        private readonly Dictionary<InternalSendMailServerParameters, Timer> _timers;
        
        public MailSenderWorkerService(ISystemDbProcess sysDb, ISettings settings, ILogger logger)
            :base(settings, logger)
        {
            _sysDb = sysDb;
            _timers = new Dictionary<InternalSendMailServerParameters, Timer>();
        }

        private Timer GetTimer(InternalSendMailServerParameters key)
        {
            Timer res = null;
            lock (_lockObjectTimer)
            {
                if (_timers.ContainsKey(key))
                    res = _timers[key];
            }
            return res;
        }


        protected override void InitializeServers()
        {
            try
            {
                Dispose();
            }
            catch
            {
                // ignored
            }

            foreach (var keyValuePair in _serverContext)
            {
                try
                {
                    var msSetting = new InternalSendMailServerParameters
                    {
                        DatabaseKey = keyValuePair.Key,
                        CheckInterval = _settings.GetSetting<int>(keyValuePair.Value, SettingConstants.MAIL_TIMEOUT_MIN),
                        ServerType = (MailServerType)_settings.GetSetting<int>(keyValuePair.Value, SettingConstants.MAIL_SERVER_TYPE),
                        FromAddress = _settings.GetSetting<string>(keyValuePair.Value, SettingConstants.MAIL_SERVER_SYSTEMMAIL),
                        Login = _settings.GetSetting<string>(keyValuePair.Value, SettingConstants.MAIL_SERVER_LOGIN),
                        Pass = _settings.GetSetting<string>(keyValuePair.Value, SettingConstants.MAIL_SERVER_PASS),
                        Server = _settings.GetSetting<string>(keyValuePair.Value, SettingConstants.MAIL_SERVER_NAME),
                        Port = _settings.GetSetting<int>(keyValuePair.Value, SettingConstants.MAIL_SERVER_PORT)
                    };

                    // start timer only once. Do not do it regulary in case we don't know how much time sending of email take. So we can continue sending only when previous iteration was comlete
                    var tmr = new Timer(CheckForNewMessages, msSetting, msSetting.CheckInterval*60000, Timeout.Infinite); 
                    _timers.Add(msSetting, tmr);
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

            var tmr = GetTimer(md);
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
                //TODO possible error: when we sent all meils, but could not save an result to DB, then all messages could be sended again
                _sysDb.MarkActionsLikeMailSended(ctx, processed);
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Error while processing new events and sending EMails", ex);
            }
            tmr.Change(md.CheckInterval * 60000, Timeout.Infinite);//start new iteration of the timer
        }


        private void SendMessage(IContext ctx, InternalSendMailParameters msSetting)
        {
            var sender = DmsResolver.Current.Kernel.Get<IMailSender>(msSetting.ServerType.ToString());

            if (sender == null) return;

            try
            {
                sender.SendMail(null, msSetting);
                Thread.Sleep(300);
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Cannot send email!", msSetting, ex);
            }
        }

        public override void Dispose()
        {
            foreach (var tmr in _timers.Values)
            {
                tmr.Change(Timeout.Infinite, Timeout.Infinite);
                tmr.Dispose();
            }
            _timers.Clear();
        }
    }
}