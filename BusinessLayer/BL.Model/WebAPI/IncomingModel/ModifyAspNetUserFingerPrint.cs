using System.ComponentModel.DataAnnotations;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetUserFingerprint : AddAspNetUserFingerprint
    {
        [Required]
        public int Id { get; set; }
    }
}