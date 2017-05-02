using System;
using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.DBModel
{
    public class AspNetUserContexts
    {
        public int Id { get; set; }

        //[Index("IX_Token")]
//        Operation failed.The index entry of length 1082 bytes for the index 'IX_Token' exceeds the maximum length of 900 bytes.
//Warning! The maximum key length is 900 bytes.The index 'IX_Token' has maximum length of 1100 bytes.For some combination of large values, the insert/update operation will fail.
//The statement has been terminated.
        [MaxLength(550)]
        public string Token { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
  
        public int ClientId { get; set; }

        [MaxLength(400)]
        public string CurrentPositionsIdList { get; set; }
        
        public DateTime LastChangeDate { get; set; }

        [MaxLength(36)]
        public string Fingerprint { get; set; }
    }
}