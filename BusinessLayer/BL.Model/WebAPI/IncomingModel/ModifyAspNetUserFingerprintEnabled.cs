using System.ComponentModel.DataAnnotations;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetUserFingerprintEnabled 
    {
        [Required]
        public bool Enabled { get; set; }
    }
}