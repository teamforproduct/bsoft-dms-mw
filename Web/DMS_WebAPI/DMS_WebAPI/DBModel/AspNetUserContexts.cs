using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_WebAPI.DBModel
{
    public class AspNetUserContexts
    {
        public int Id { get; set; }

        //[Index("IX_Token", 1)]
        [MaxLength(2000)]
        public string Token { get; set; }

        public string UserId { get; set; }
        public int ClientId { get; set; }
        [MaxLength(2000)]
        public string CurrentPositionsIdList { get; set; }
        public int DatabaseId { get; set; }
        public bool IsChangePasswordRequired { get; set; }
        public int? LoginLogId { get; set; }

        [MaxLength(2000)]
        public string LoginLogInfo { get; set; }
    }
}