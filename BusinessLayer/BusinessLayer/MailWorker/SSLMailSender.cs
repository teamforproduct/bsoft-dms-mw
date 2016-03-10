using System.Linq;
using System.Web.Mail;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.MailWorker
{
    public class SSLMailSender: BaseMailSender
    {
        protected override void Send(InternalSendMailParameters mailData)
        {
            var myMail = new MailMessage();
            myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", mailData.Server);
            myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", mailData.Port);
            myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", "2");
            //sendusing: cdoSendUsingPort, value 2, for sending the message using 
            //the network.

            //smtpauthenticate: Specifies the mechanism used when authenticating 
            //to an SMTP 
            //service over the network. Possible values are:
            //- cdoAnonymous, value 0. Do not authenticate.
            //- cdoBasic, value 1. Use basic clear-text authentication. 
            //When using this option you have to provide the user name and password 
            //through the sendusername and sendpassword fields.
            //- cdoNTLM, value 2. The current process security context is used to 
            // authenticate with the service.
            myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
            //Use 0 for anonymous
            myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mailData.Login);
            myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", mailData.Pass);
            myMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");
            myMail.From = mailData.FromAddress;
            myMail.To = mailData.ToAddress;
            myMail.Subject = mailData.Subject;
            myMail.BodyFormat = MailFormat.Html;
            myMail.Body = mailData.Body;
            myMail.Priority = MailPriority.High;

            foreach (var filePath in mailData.AttachedFiles.Where(filePath => !string.IsNullOrEmpty(filePath.Trim())))
            {
                MailAttachment MyAttachment = new MailAttachment(filePath.Trim());
                myMail.Attachments.Add(MyAttachment);
            }

            SmtpMail.SmtpServer = $"{mailData.Server}:{mailData.Port}";
            SmtpMail.Send(myMail);
        }
    }
}