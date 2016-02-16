using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    /// <summary>
    /// Модель должности, от которой будет выполнятся действие
    /// </summary>
    public class CurrentPosition
    {
        /// <summary>
        /// ИД должности, от которой будет выполнятся действие
        /// </summary>
        [Required]
        public int CurrentPositionId { get; set; }
//        public string Name { get; set; }
    }
}