using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.MailWorker
{
    public interface IMailSender
    {
        //event Action<object, Exception> OnErrorMailSend;
        //event Action<object> OnSuccessMailSend;

        void SendMail(object senderId, InternalSendMailParameters mailData);
    }
}