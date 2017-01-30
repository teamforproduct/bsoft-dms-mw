using BL.Model.Database;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.WebAPI.IncomingModel
{
    public class AddAspNetUserFingerprint
    {
        [IgnoreDataMember]
        public string UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Fingerprint { get; set; }
        [IgnoreDataMember]
        public string Browser { get; set; }
        [IgnoreDataMember]
        public string Platform { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}