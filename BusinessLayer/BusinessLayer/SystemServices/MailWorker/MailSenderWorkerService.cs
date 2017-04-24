using System;
using System.Collections.Generic;
using System.Threading;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.SystemDb;
using BL.Model.SystemCore.InternalModel;
using Ninject;
using Ninject.Parameters;
using BL.Model.Enums;
using BL.CrossCutting.Helpers;

namespace BL.Logic.SystemServices.MailWorker
{
    public class MailSenderWorkerService : BaseSystemWorkerService, IMailSenderWorkerService
    {
        private readonly ISystemDbProcess _sysDb;

        private readonly Dictionary<InternalSendMailServerParameters, Timer> _timers;
        
        public MailSenderWorkerService(ISystemDbProcess sysDb, ISettingValues settingValues, ILogger logger)
            :base(settingValues, logger)
        {
            _sysDb = sysDb;
            _timers = new Dictionary<InternalSendMailServerParameters, Timer>();
        }

        private Timer GetTimer(InternalSendMailServerParameters key)
        {
            Timer res = null;
            lock (LockObjectTimer)
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

            foreach (var keyValuePair in ServerContext)
            {
                try
                {
                    var ctx = keyValuePair.Value;

                    var msSetting = GetMailServerParameters(MailServers.Noreply, keyValuePair.Key);

                    // start timer only once. Do not do it regulary in case we don't know how much time sending of email take. So we can continue sending only when previous iteration was comlete
                    var tmr = new Timer(CheckForNewMessages, msSetting, msSetting.CheckInterval*60000, Timeout.Infinite); 
                    _timers.Add(msSetting, tmr);
                }
                catch (Exception ex)
                {
                    Logger.Error(keyValuePair.Value, "Could not start MeilSender for server", ex);
                }
            }
        }

        private InternalSendMailServerParameters GetMailServerParameters(MailServers server, string databaseKey = "") 
        {
            switch (server)
            {
                case MailServers.Docum:
                    return new InternalSendMailServerParameters
                    {
                        DatabaseKey = databaseKey,
                        CheckInterval = SettingValues.GetMailDocumSenderTimeoutMin(),
                        ServerType = SettingValues.GetMailDocumServerType(),
                        Server = SettingValues.GetMailDocumServerName(),
                        Port = SettingValues.GetMailDocumServerPort(),
                        FromAddress = SettingValues.GetMailDocumEmail(),
                        Login = SettingValues.GetMailDocumLogin(),
                        Pass = SettingValues.GetMailDocumPassword(),
                    };
                case MailServers.Noreply:
                    return new InternalSendMailServerParameters
                    {
                        DatabaseKey = databaseKey,
                        CheckInterval = SettingValues.GetMailNoreplySenderTimeoutMin(),
                        ServerType = SettingValues.GetMailNoreplyServerType(),
                        Server = SettingValues.GetMailNoreplyServerName(),
                        Port = SettingValues.GetMailNoreplyServerPort(),
                        FromAddress = SettingValues.GetMailNoreplyEmail(),
                        Login = SettingValues.GetMailNoreplyLogin(),
                        Pass = SettingValues.GetMailNoreplyPassword(),
                    };
                case MailServers.SMS:
                    return new InternalSendMailServerParameters
                    {
                        DatabaseKey = databaseKey,
                        CheckInterval = SettingValues.GetMailSMSSenderTimeoutMin(),
                        ServerType = SettingValues.GetMailSMSServerType(),
                        Server = SettingValues.GetMailSMSServerName(),
                        Port = SettingValues.GetMailSMSServerPort(),
                        FromAddress = SettingValues.GetMailSMSEmail(),
                        Login = SettingValues.GetMailSMSLogin(),
                        Pass = SettingValues.GetMailSMSPassword(),
                    };
            }

            return null;
        }


        public void CheckForNewMessages(object state)
        {
            
            var md = state as InternalSendMailServerParameters;

            if (md == null) return;

            var tmr = GetTimer(md);
            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            ctx.DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", ctx.CurrentDB));
            try
            {
                var processed = new InternalMailProcessed();
                var newEvents = _sysDb.GetNewActionsForMailing(ctx);
                foreach (var evt in newEvents)
                {
                    try
                    {
                        var mailParam = new InternalSendMailParameters(md);
                        //TODO make correct subject and body!
                        mailParam.ToAddress = evt.DestinationAgentEmail;
                        mailParam.Subject = "Automatic notification";
                        mailParam.Body = "You have new event: " + evt.Description;
                        SendMessage(ctx, mailParam);
                        processed.ProcessedEventIds.Add(evt.EventId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ctx, $"MailWorkerService cannot process Event Id={evt.EventId} DocId ={evt.DocumentId} ", ex);
                    }
                }
                //TODO possible error: when we sent all meils, but could not save an result to DB, then all messages could be sended again
                _sysDb.MarkActionsLikeMailSended(ctx, processed);
            }
            catch (Exception ex)
            {
                Logger.Error(ctx, "Error while processing new events and sending EMails", ex);
            }
            ((DmsContext)ctx.DbContext).Dispose();
            ctx.DbContext = null;
            tmr.Change(md.CheckInterval * 60000, Timeout.Infinite);//start new iteration of the timer
        }


        public void SendMessage(IContext context, InternalSendMailParameters msSetting)
        {
            var sender = DmsResolver.Current.Kernel.Get<IMailSender>(msSetting.ServerType.ToString());
            try
            {
                sender.SendMail(null, msSetting);
                Thread.Sleep(300);
            }
            catch (Exception ex)
            {
                // Приходится высылать почту еще до создания клиента и клиентской базы
                if (context != null) Logger.Error(context, "Cannot send email!", msSetting, ex);
            }
        }

        public void SendMessage(IContext context, MailServers server, string toAddress, string subject, string body)
        {
            var msSetting = new InternalSendMailParameters(GetMailServerParameters(server)
                    )
            {
                Body = body,
                ToAddress = toAddress,
                Subject = subject,
            };
            SendMessage(context, msSetting);
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