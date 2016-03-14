using BL.Model.Enums;

namespace BL.Model.SystemCore.InternalModel
{
    /// <summary>
    /// Mail Server parameters
    /// </summary>
    public class InternalSendMailServerParameters
    {
        /// <summary>
        /// string database key for which DB that parameters is
        /// </summary>
        public string DatabaseKey { get; set; }

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
        /// Type of the connection to server
        /// </summary>
        public MailServerType ServerType { get; set; }

        /// <summary>
        /// Sender email address
        /// </summary> 
        public string FromAddress { get; set; }

        /// <summary>
        /// Time in minutes how often check the new events for mail
        /// </summary>
        public int CheckInterval { get; set; }
    }
}