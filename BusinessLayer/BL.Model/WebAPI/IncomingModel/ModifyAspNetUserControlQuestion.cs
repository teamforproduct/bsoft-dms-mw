using System.ComponentModel.DataAnnotations;

namespace BL.Model.WebAPI.IncomingModel
{
    public class ModifyAspNetUserControlQuestion
    {
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public string Answer { get; set; }
    }
}