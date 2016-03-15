using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.SystemServices.MailWorker
{
    public abstract class BaseMailSender : IMailSender
    {
        //public event Action<object> OnSuccessMailSend;
        //public event Action<object, Exception> OnErrorMailSend;

        protected abstract void Send(InternalSendMailParameters mailData);

        public void SendMail(object senderId, InternalSendMailParameters mailData)
        {
            Send(mailData);

            //try
            //{
            //    Send(mailData);
            //    OnSuccessMailSend?.Invoke(senderId);
            //}
            //catch (Exception ex)
            //{
            //    OnErrorMailSend?.Invoke(senderId, ex);
            //}
        }
    }
}