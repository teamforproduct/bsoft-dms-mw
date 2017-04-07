using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddTag 
    {
        [Required]
        public string Name { get; set; }
        public string Color { get; set; }
        [Required]
        public bool IsActive { get; set; }

    }
}
