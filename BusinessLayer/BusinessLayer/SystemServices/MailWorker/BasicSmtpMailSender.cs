using System.Linq;
using System.Net.Mail;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.SystemServices.MailWorker
{
    public class BasicSmtpMailSender : BaseMailSender
    {
        protected override void Send(InternalSendMailParameters mailData)
        {
            using (SmtpClient client = new SmtpClient(mailData.Server, mailData.Port))
            {
                client.Credentials = new System.Net.NetworkCredential(mailData.Login, mailData.Pass);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 7000;
                // Specify the e-mail sender.
                // Create a mailing address that includes a UTF8 character
                // in the display name.
                MailAddress from = new MailAddress(mailData.FromAddress, mailData.DisplayFromName,
                    System.Text.Encoding.UTF8);
                // Set destinations for the e-mail message.
                MailAddress to = new MailAddress(mailData.ToAddress);
                // Specify the message content.
                MailMessage message = new MailMessage(from, to);
                message.Body = mailData.Body;
                // Include some non-ASCII characters in body and subject.
                //string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
                //message.Body += Environment.NewLine + someArrows;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = mailData.Subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                if (mailData.Files != null)
                {
                    foreach (var filePath in mailData.Files.Where(filePath => !string.IsNullOrEmpty(filePath.Trim())))
                    {
                        message.Attachments.Add(new Attachment(filePath.Trim()));
                    }
                }

                // Set the method that is called back when the send operation ends.
                //client.SendCompleted += ClientOnSendCompleted;
                // The userState can be any object that allows your callback 
                // method to identify this send operation.
                // For this example, the userToken is a string constant.
                //string userState = "test message1";
                client.Send(message);

            }
        }
    }
}