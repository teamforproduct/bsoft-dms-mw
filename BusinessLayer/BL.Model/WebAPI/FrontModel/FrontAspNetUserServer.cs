using BL.Model.Database;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontAspNetUserServer
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string UserId { get; set; }
        public int ClientId { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string ClientName { get; set; }
    }
}