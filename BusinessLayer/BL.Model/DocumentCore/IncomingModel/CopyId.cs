using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class CopyId 
    {
        /// <summary>
        /// ИД для создания копии
        /// </summary>
        [Required]
        public int Id { get; set; }
    }
}
