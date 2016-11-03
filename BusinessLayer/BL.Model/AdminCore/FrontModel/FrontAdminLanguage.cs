using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// Модель языка
    /// </summary>
    public class FrontAdminLanguage
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        public string Code { get; set; }
        /// <summary>
        /// Название языка
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Язык по умолчанию
        /// </summary>
        public bool? IsDefault { get; set; }
    }
}