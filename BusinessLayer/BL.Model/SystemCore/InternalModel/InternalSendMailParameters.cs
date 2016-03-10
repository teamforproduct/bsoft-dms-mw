
namespace BL.Model.SystemCore.InternalModel
{
    /// <summary>
    /// Describe data for sending mail
    /// </summary>
    public class InternalSendMailParameters: InternalSendMailServerParameters
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public InternalSendMailParameters()
        {
        }


        /// <summary>
        /// Create copy of instance
        /// </summary>
        /// <param name="md"></param>
        public InternalSendMailParameters(InternalSendMailServerParameters md)
        {
            Login = md.Login;
            Pass = md.Pass;
            Server = md.Server;
            Port = md.Port;
            ServerType = md.ServerType;
            FromAddress = md.FromAddress;
        }
        
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



    }
}