using BL.Model.Database;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontAdminServerByUser
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        public string Name { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
    }
}