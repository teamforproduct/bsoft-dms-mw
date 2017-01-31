using BL.Model.Database;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

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