using System;
using BL.Model.SystemCore;

namespace BL.Logic.MailWorker
{
    public abstract class BaseMailSender : IMailSender
    {
        public event Action<object> OnSuccessMailSend;
        public event Action<object, Exception> OnErrorMailSend;

        protected abstract void Send(SendMailData mailData);

        public void SendMail(object senderId, SendMailData mailData)
        {
            try
            {
                Send(mailData);
                OnSuccessMailSend?.Invoke(senderId);
            }
            catch (Exception ex)
            {
                OnErrorMailSend?.Invoke(senderId, ex);
            }
        }
    }
}