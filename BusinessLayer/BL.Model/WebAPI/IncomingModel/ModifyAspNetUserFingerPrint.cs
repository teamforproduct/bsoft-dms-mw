using System.ComponentModel.DataAnnotations;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetUserFingerPrint : AddAspNetUserFingerprint
    {
        [Required]
        public int Id { get; set; }
    }
}