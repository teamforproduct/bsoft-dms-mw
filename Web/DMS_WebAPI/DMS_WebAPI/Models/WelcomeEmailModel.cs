namespace DMS_WebAPI.Models
{
    public class WelcomeEmailModel : BaseMailModel
    {
        public string InvitingName { get; set; }

        public string ClientName{ get; set; }
    }
}