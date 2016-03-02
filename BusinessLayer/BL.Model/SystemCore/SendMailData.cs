using BL.Model.Enums;

namespace BL.Model.SystemCore
{
    /// <summary>
    /// Describe data for sending mail
    /// </summary>
    public class SendMailData
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public SendMailData()
        {
        }


        /// <summary>
        /// Create copy of instance
        /// </summary>
        /// <param name="md"></param>
        public SendMailData(SendMailData md)
        {
            Login = md.Login;
            Pass = md.Pass;
            Server = md.Server;
            Port = md.Port;
            ServerType = md.ServerType;
            FromAddress = md.FromAddress;
        }

        /// <summary>
        /// Server address
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Server port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Login for connection to SMTP server
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Password for connection to SMTP server
        /// </summary>
        public string Pass { get; set; }
        /// <summary>
        /// Sender email address
        /// </summary>
        public string FromAddress { get; set; }
        /// <summary>
        /// Recepient (To) email address
        /// </summary>
        public string ToAddress { get; set; }
        /// <summary>
        /// Mail subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Mail body
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Name to display instead of From email
        /// </summary>
        public string DisplayFromName { get; set; }
        /// <summary>
        /// List of path to attached files.
        /// </summary>
        public string[] AttachedFiles { get; set; }

        /// <summary>
        /// Type of the connection to server
        /// </summary>
        public MailServerType ServerType { get; set; }
    }
}