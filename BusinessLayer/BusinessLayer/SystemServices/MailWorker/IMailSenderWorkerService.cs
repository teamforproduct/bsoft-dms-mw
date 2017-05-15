using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.MailWorker
{
    public interface IMailSenderWorkerService
    {
        void CheckForNewMessages(IContext ctx, InternalSendMailServerParameters md);
        InternalSendMailServerParameters GetMailServerParameters(MailServers server, string databaseKey = "");
        void SendMessage(IContext context, InternalSendMailParameters msSetting);
        void SendMessage(IContext context, MailServers server, string toAddress, string subject, string body);
    }
}