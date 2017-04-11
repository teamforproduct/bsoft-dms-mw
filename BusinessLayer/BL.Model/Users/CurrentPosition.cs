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
        public int? CurrentPositionId { get; set; }

    }
}