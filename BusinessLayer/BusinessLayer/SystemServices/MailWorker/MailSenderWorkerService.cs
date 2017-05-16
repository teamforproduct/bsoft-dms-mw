using System;
using System.Threading;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Model.SystemCore.InternalModel;
using Ninject;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.MailWorker
{
    public class MailSenderWorkerService :  IMailSenderWorkerService
    {
        private readonly ISystemDbProcess _sysDb;
        protected readonly ISettingValues SettingValues;
        protected readonly ILogger Logger;

        public MailSenderWorkerService(ISystemDbProcess sysDb, ISettingValues settingValues, ILogger logger)
        {
            _sysDb = sysDb;
            SettingValues = settingValues;
            Logger = logger;
        }

        public InternalSendMailServerParameters GetMailServerParameters(MailServers server, string databaseKey = "") 
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

        public void CheckForNewMessages(IContext ctx, InternalSendMailServerParameters md)
        {
          
            if (md == null || ctx == null) return;
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
            var msSetting = new InternalSendMailParameters(GetMailServerParameters(server))
            {
                Body = body,
                ToAddress = toAddress,
                Subject = subject,
            };
            SendMessage(context, msSetting);
        }
    }
}