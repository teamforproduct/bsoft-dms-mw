using System;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.SystemServices.MailWorker
{
    public interface IMailSenderWorkerService: ISystemWorkerService, IDisposable
    {
        void CheckForNewMessages(object state);
        void SendMessage(IContext ctx, InternalSendMailParameters msSetting);
        void SendMessage(IContext ctx, string toAddress, string subject, string body);
    }
}