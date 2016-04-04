using BL.Model.Database;

namespace BL.Model.AspNet.FrontModel
{
    public class FrontAspNetUserServers
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string UserId { get; set; }
        public string ServerName { get; set; }
        public string ClientName { get; set; }
    }
}