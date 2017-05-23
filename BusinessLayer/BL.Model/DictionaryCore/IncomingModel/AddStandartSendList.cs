using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddStandartSendList
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int PositionId { get; set; }
        
    }
}
