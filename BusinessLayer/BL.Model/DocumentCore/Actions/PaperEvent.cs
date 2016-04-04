using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель события по бумажному носителю
    /// </summary>
    public class PaperEvent
    {
        /// <summary>
        /// ИД БН
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Описание события
        /// </summary>
        public string Description { get; set; }
    }
}
