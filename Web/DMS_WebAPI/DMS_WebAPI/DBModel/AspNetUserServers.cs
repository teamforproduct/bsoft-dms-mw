using DMS_WebAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetUserServers
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int ServerId { get; set; }
        [ForeignKey("ServerId")]
        public virtual AdminServers Server { get; set; }
}
}
